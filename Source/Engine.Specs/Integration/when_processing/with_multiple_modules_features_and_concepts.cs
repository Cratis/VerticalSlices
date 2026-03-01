// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing;

/// <summary>
/// <para>
/// Comprehensive end-to-end scenario exercising the full engine pipeline across three
/// independent modules, each with a varying number of features containing different
/// slice types and domain concepts declared at every scope level.
/// </para>
/// <para>Module layout:</para>
/// <para>
/// <b>Healthcare</b> (concepts: PatientId, Email)
///   └ Registration (concept: FullName)
///       └ RegisterPatient (StateChange) — properties use FullName, Email concepts
///       └ PatientView (StateView) — read model with concept-typed properties
///   └ Appointments (concept: AppointmentId)
///       └ Scheduling
///           └ ScheduleAppointment (StateChange) — uses PatientId, AppointmentId
///           └ CancelAppointment (StateChange)
///       └ Calendar
///           └ SyncCalendar (Translator)
/// </para>
/// <para>
/// <b>Billing</b> (concepts: InvoiceId, Amount)
///   └ Invoicing (concept: InvoiceNumber)
///       └ CreateInvoice (StateChange) — uses InvoiceNumber, Amount
///       └ InvoiceOverview (StateView) — uses Amount, InvoiceNumber
///       └ SendReminder (Automation) — produces InvoiceReminded event using InvoiceId
///   └ Payments
///       └ RecordPayment (StateChange) — uses InvoiceId, Amount
/// </para>
/// <para>
/// <b>Notifications</b> (concept: NotificationId)
///   └ Email
///       └ IngestDelivery (Translator) — external → internal event
///       └ DeliveryLog (StateView) — read model with NotificationId concept
///   └ Push (concept: DeviceToken)
///       └ SendPush (StateChange) — uses NotificationId from parent, DeviceToken from own feature
/// </para>
/// <para>
/// The test writes all generated files to disk and compiles them with <c>dotnet build</c>,
/// verifying that concept-as-property-type resolution produces correct cross-namespace
/// <c>using</c> directives and compilable output.
/// </para>
/// </summary>
public class with_multiple_modules_features_and_concepts : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        // ═══════════════════════════════════════════════════════════════════════
        //  Module 1: Healthcare
        // ═══════════════════════════════════════════════════════════════════════

        // Module-level concepts
        var patientId = new Concept("PatientId", "Guid", "Uniquely identifies a patient", [], IsEventSourceId: true);
        var email = new Concept(
            "Email",
            "string",
            "An email address",
            [
                new ConceptValidationRule(ConceptValidationRuleType.NotEmpty),
                new ConceptValidationRule(ConceptValidationRuleType.EmailAddress)
            ]);

        // Feature: Registration — concept: FullName
        var fullName = new Concept(
            "FullName",
            "string",
            "A person's full name",
            [
                new ConceptValidationRule(ConceptValidationRuleType.NotEmpty),
                new ConceptValidationRule(ConceptValidationRuleType.MaximumLength, "200")
            ]);

        var patientRegistered = new EventType(
            "PatientRegistered",
            "A patient was registered in the system",
            [
                new Property("PatientId", "PatientId"),
                new Property("Name", "FullName"),
                new Property("ContactEmail", "Email"),
                new Property("DateOfBirth", "DateTimeOffset")
            ]);

        var registerPatient = new Command(
            "RegisterPatient",
            "Registers a new patient",
            [
                new Property("Name", "FullName"),
                new Property("ContactEmail", "Email"),
                new Property("DateOfBirth", "DateTimeOffset")
            ],
            "PatientId");

        var registerPatientSlice = new VerticalSlice(
            "RegisterPatient",
            VerticalSliceType.StateChange,
            "Captures a new patient registration and emits the PatientRegistered event.",
            null,
            [registerPatient],
            [],
            [patientRegistered]);

        // StateView — PatientView
        var patientIdMappings = new[] { new EventPropertyMapping("PatientRegistered", EventPropertyMappingKind.Set, "PatientId") };
        var nameMappings = new[] { new EventPropertyMapping("PatientRegistered", EventPropertyMappingKind.Set, "Name") };
        var emailMappings = new[] { new EventPropertyMapping("PatientRegistered", EventPropertyMappingKind.Set, "ContactEmail") };
        var registeredAtMappings = new[] { new EventPropertyMapping("PatientRegistered", EventPropertyMappingKind.SetFromContext, "Occurred") };

        var patientSummary = new ReadModel(
            "PatientSummary",
            "A summary of a registered patient",
            [
                new ReadModelProperty("PatientId", "PatientId", patientIdMappings),
                new ReadModelProperty("Name", "FullName", nameMappings),
                new ReadModelProperty("ContactEmail", "Email", emailMappings),
                new ReadModelProperty("RegisteredAt", "DateTimeOffset", registeredAtMappings)
            ]);

        var patientViewSlice = new VerticalSlice(
            "PatientView",
            VerticalSliceType.StateView,
            null,
            null,
            [],
            [patientSummary],
            [patientRegistered]);

        var registrationFeature = new Feature(
            "Registration",
            [fullName],
            [],
            [registerPatientSlice, patientViewSlice]);

        // Feature: Appointments — concept: AppointmentId
        var appointmentId = new Concept("AppointmentId", "Guid", "Uniquely identifies an appointment", [], IsEventSourceId: true);

        var appointmentScheduled = new EventType(
            "AppointmentScheduled",
            "An appointment was scheduled",
            [
                new Property("AppointmentId", "AppointmentId"),
                new Property("PatientId", "PatientId"),
                new Property("DoctorName", "string"),
                new Property("ScheduledAt", "DateTimeOffset")
            ]);

        var appointmentCancelled = new EventType(
            "AppointmentCancelled",
            "An appointment was cancelled",
            [new Property("AppointmentId", "AppointmentId")]);

        var scheduleAppointment = new Command(
            "ScheduleAppointment",
            "Schedules a new appointment",
            [
                new Property("PatientId", "PatientId"),
                new Property("DoctorName", "string"),
                new Property("ScheduledAt", "DateTimeOffset")
            ],
            "AppointmentId");

        var scheduleSlice = new VerticalSlice(
            "ScheduleAppointment",
            VerticalSliceType.StateChange,
            null,
            null,
            [scheduleAppointment],
            [],
            [appointmentScheduled]);

        var cancelAppointment = new Command("CancelAppointment", "Cancels an appointment", [], "AppointmentId");

        var cancelSlice = new VerticalSlice(
            "CancelAppointment",
            VerticalSliceType.StateChange,
            null,
            null,
            [cancelAppointment],
            [],
            [appointmentCancelled]);

        var schedulingSubFeature = new Feature("Scheduling", [], [], [scheduleSlice, cancelSlice]);

        // Translator — SyncCalendar
        var externalCalendarEvent = new EventType(
            "ExternalCalendarEvent",
            "An event from the external calendar system",
            [
                new Property("ExternalRef", "string"),
                new Property("AppointmentId", "AppointmentId")
            ],
            EventKind.External);

        var calendarEventSynced = new EventType(
            "CalendarEventSynced",
            "An external calendar event was synced",
            [
                new Property("AppointmentId", "AppointmentId"),
                new Property("ExternalRef", "string")
            ]);

        var syncCalendarSlice = new VerticalSlice(
            "SyncCalendar",
            VerticalSliceType.Translator,
            null,
            null,
            [],
            [],
            [externalCalendarEvent, calendarEventSynced]);

        var calendarSubFeature = new Feature("Calendar", [], [], [syncCalendarSlice]);

        var appointmentsFeature = new Feature(
            "Appointments",
            [appointmentId],
            [schedulingSubFeature, calendarSubFeature],
            []);

        var healthcareModule = new Module("Healthcare", [patientId, email], [registrationFeature, appointmentsFeature]);

        // ═══════════════════════════════════════════════════════════════════════
        //  Module 2: Billing
        // ═══════════════════════════════════════════════════════════════════════

        // Module-level concepts
        var invoiceId = new Concept("InvoiceId", "Guid", "Uniquely identifies an invoice", [], IsEventSourceId: true);
        var amount = new Concept("Amount", "decimal", "A monetary amount", []);

        // Feature: Invoicing — concept: InvoiceNumber
        var invoiceNumber = new Concept(
            "InvoiceNumber",
            "string",
            "A human-readable invoice number",
            [new ConceptValidationRule(ConceptValidationRuleType.NotEmpty)]);

        var invoiceCreated = new EventType(
            "InvoiceCreated",
            "An invoice was created",
            [
                new Property("InvoiceId", "InvoiceId"),
                new Property("InvoiceNumber", "InvoiceNumber"),
                new Property("TotalAmount", "Amount"),
                new Property("CustomerId", "string")
            ]);

        var createInvoice = new Command(
            "CreateInvoice",
            "Creates a new invoice",
            [
                new Property("InvoiceNumber", "InvoiceNumber"),
                new Property("TotalAmount", "Amount"),
                new Property("CustomerId", "string")
            ],
            "InvoiceId");

        var createInvoiceSlice = new VerticalSlice(
            "CreateInvoice",
            VerticalSliceType.StateChange,
            null,
            null,
            [createInvoice],
            [],
            [invoiceCreated]);

        // StateView — InvoiceOverview
        var invIdMappings = new[] { new EventPropertyMapping("InvoiceCreated", EventPropertyMappingKind.Set, "InvoiceId") };
        var invNumberMappings = new[] { new EventPropertyMapping("InvoiceCreated", EventPropertyMappingKind.Set, "InvoiceNumber") };
        var invAmountMappings = new[] { new EventPropertyMapping("InvoiceCreated", EventPropertyMappingKind.Set, "TotalAmount") };
        var invCustomerMappings = new[] { new EventPropertyMapping("InvoiceCreated", EventPropertyMappingKind.Set, "CustomerId") };

        var invoiceOverview = new ReadModel(
            "InvoiceOverview",
            "Overview of a created invoice",
            [
                new ReadModelProperty("InvoiceId", "InvoiceId", invIdMappings),
                new ReadModelProperty("InvoiceNumber", "InvoiceNumber", invNumberMappings),
                new ReadModelProperty("TotalAmount", "Amount", invAmountMappings),
                new ReadModelProperty("CustomerId", "string", invCustomerMappings)
            ]);

        var invoiceOverviewSlice = new VerticalSlice(
            "InvoiceOverview",
            VerticalSliceType.StateView,
            null,
            null,
            [],
            [invoiceOverview],
            [invoiceCreated]);

        // Automation — SendReminder
        var invoiceReminded = new EventType(
            "InvoiceReminded",
            "A payment reminder was sent for an invoice",
            [
                new Property("InvoiceId", "InvoiceId"),
                new Property("ReminderCount", "int")
            ]);

        var unpaidInvIdMappings = new[] { new EventPropertyMapping("InvoiceCreated", EventPropertyMappingKind.Set, "InvoiceId") };
        var unpaidAmountMappings = new[] { new EventPropertyMapping("InvoiceCreated", EventPropertyMappingKind.Set, "TotalAmount") };

        var unpaidInvoice = new ReadModel(
            "UnpaidInvoice",
            "An invoice that has not been paid yet",
            [
                new ReadModelProperty("InvoiceId", "InvoiceId", unpaidInvIdMappings),
                new ReadModelProperty("TotalAmount", "Amount", unpaidAmountMappings)
            ]);

        var sendReminder = new Command("SendReminder", "Sends a payment reminder", [], "InvoiceId");

        var sendReminderSlice = new VerticalSlice(
            "SendReminder",
            VerticalSliceType.Automation,
            null,
            null,
            [sendReminder],
            [unpaidInvoice],
            [invoiceReminded]);

        var invoicingFeature = new Feature(
            "Invoicing",
            [invoiceNumber],
            [],
            [createInvoiceSlice, invoiceOverviewSlice, sendReminderSlice]);

        // Feature: Payments — no feature-level concepts
        var paymentRecorded = new EventType(
            "PaymentRecorded",
            "A payment was recorded against an invoice",
            [
                new Property("InvoiceId", "InvoiceId"),
                new Property("PaidAmount", "Amount"),
                new Property("PaidAt", "DateTimeOffset")
            ]);

        var recordPayment = new Command(
            "RecordPayment",
            "Records a payment against an invoice",
            [
                new Property("PaidAmount", "Amount"),
                new Property("PaidAt", "DateTimeOffset")
            ],
            "InvoiceId");

        var recordPaymentSlice = new VerticalSlice(
            "RecordPayment",
            VerticalSliceType.StateChange,
            null,
            null,
            [recordPayment],
            [],
            [paymentRecorded]);

        var paymentsFeature = new Feature("Payments", [], [], [recordPaymentSlice]);

        var billingModule = new Module("Billing", [invoiceId, amount], [invoicingFeature, paymentsFeature]);

        // ═══════════════════════════════════════════════════════════════════════
        //  Module 3: Notifications
        // ═══════════════════════════════════════════════════════════════════════

        // Module-level concept
        var notificationId = new Concept("NotificationId", "Guid", "Uniquely identifies a notification", [], IsEventSourceId: true);

        // Feature: Email — Translator + StateView
        var externalEmailDelivered = new EventType(
            "ExternalEmailDelivered",
            "Email delivery confirmation from external provider",
            [
                new Property("MessageId", "string"),
                new Property("Recipient", "string")
            ],
            EventKind.External);

        var emailDelivered = new EventType(
            "EmailDelivered",
            "An email was confirmed as delivered internally",
            [
                new Property("NotificationId", "NotificationId"),
                new Property("Recipient", "string")
            ]);

        var ingestDeliverySlice = new VerticalSlice(
            "IngestDelivery",
            VerticalSliceType.Translator,
            null,
            null,
            [],
            [],
            [externalEmailDelivered, emailDelivered]);

        var delivNotifIdMappings = new[] { new EventPropertyMapping("EmailDelivered", EventPropertyMappingKind.Set, "NotificationId") };
        var delivRecipientMappings = new[] { new EventPropertyMapping("EmailDelivered", EventPropertyMappingKind.Set, "Recipient") };
        var delivAtMappings = new[] { new EventPropertyMapping("EmailDelivered", EventPropertyMappingKind.SetFromContext, "Occurred") };

        var deliveryRecord = new ReadModel(
            "DeliveryRecord",
            "A record of a delivered email notification",
            [
                new ReadModelProperty("NotificationId", "NotificationId", delivNotifIdMappings),
                new ReadModelProperty("Recipient", "string", delivRecipientMappings),
                new ReadModelProperty("DeliveredAt", "DateTimeOffset", delivAtMappings)
            ]);

        var deliveryLogSlice = new VerticalSlice(
            "DeliveryLog",
            VerticalSliceType.StateView,
            null,
            null,
            [],
            [deliveryRecord],
            [emailDelivered]);

        var emailFeature = new Feature("Email", [], [], [ingestDeliverySlice, deliveryLogSlice]);

        // Feature: Push — concept: DeviceToken
        var deviceToken = new Concept(
            "DeviceToken",
            "string",
            "A push notification device token",
            [new ConceptValidationRule(ConceptValidationRuleType.NotEmpty)]);

        var pushNotificationSent = new EventType(
            "PushNotificationSent",
            "A push notification was sent to a device",
            [
                new Property("NotificationId", "NotificationId"),
                new Property("DeviceToken", "DeviceToken"),
                new Property("Title", "string"),
                new Property("Body", "string")
            ]);

        var sendPush = new Command(
            "SendPush",
            "Sends a push notification",
            [
                new Property("DeviceToken", "DeviceToken"),
                new Property("Title", "string"),
                new Property("Body", "string")
            ],
            "NotificationId");

        var sendPushSlice = new VerticalSlice(
            "SendPush",
            VerticalSliceType.StateChange,
            null,
            null,
            [sendPush],
            [],
            [pushNotificationSent]);

        var pushFeature = new Feature("Push", [deviceToken], [], [sendPushSlice]);

        var notificationsModule = new Module("Notifications", [notificationId], [emailFeature, pushFeature]);

        _modules = [healthcareModule, billingModule, notificationsModule];
    }

    async Task Because()
    {
        await _engine.Process(_modules);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromRenderedArtifacts();
        _buildExitCode = await RunDotnet("build");
    }

    // ── Healthcare module ──────────────────────────────────────────────────────

    [Fact] void should_generate_healthcare_module_concept_files() =>
        new[] { "PatientId.cs", "Email.cs" }
            .All(name => _generatedFiles.Any(f => f.ArtifactPath.Contains("Healthcare") && f.ArtifactPath.EndsWith(name)))
            .ShouldBeTrue();

    [Fact] void should_generate_registration_feature_concept_files() =>
        _generatedFiles.Any(f => f.ArtifactPath.Contains("Registration") && f.ArtifactPath.EndsWith("FullName.cs"))
            .ShouldBeTrue();

    [Fact] void should_generate_appointments_feature_concept_file() =>
        _generatedFiles.Any(f => f.ArtifactPath.Contains("Appointments") && f.ArtifactPath.EndsWith("AppointmentId.cs")).ShouldBeTrue();

    [Fact] void should_generate_register_patient_slice_file() =>
        _generatedFiles.Any(f => f.ArtifactPath.EndsWith("RegisterPatient.cs")).ShouldBeTrue();

    [Fact] void should_generate_patient_view_file() =>
        _generatedFiles.Any(f => f.ArtifactPath.EndsWith("PatientView.cs")).ShouldBeTrue();

    [Fact] void should_generate_scheduling_slice_files() =>
        new[] { "ScheduleAppointment.cs", "CancelAppointment.cs" }
            .All(name => _generatedFiles.Any(f => f.ArtifactPath.EndsWith(name)))
            .ShouldBeTrue();

    [Fact] void should_generate_translator_file() =>
        _generatedFiles.Any(f => f.ArtifactPath.EndsWith("SyncCalendar.cs")).ShouldBeTrue();

    [Fact] void should_not_generate_translator_external_events() =>
        _generatedFiles.Any(f => f.ArtifactPath.EndsWith("ExternalCalendarEvent.cs")).ShouldBeFalse();

    // ── Billing module ─────────────────────────────────────────────────────────

    [Fact] void should_generate_billing_module_concept_files() =>
        new[] { "InvoiceId.cs", "Amount.cs" }
            .All(name => _generatedFiles.Any(f => f.ArtifactPath.Contains("Billing") && f.ArtifactPath.EndsWith(name)))
            .ShouldBeTrue();

    [Fact] void should_generate_invoicing_feature_concept_files() =>
        _generatedFiles.Any(f => f.ArtifactPath.Contains("Invoicing") && f.ArtifactPath.EndsWith("InvoiceNumber.cs"))
            .ShouldBeTrue();

    [Fact] void should_generate_create_invoice_slice_file() =>
        _generatedFiles.Any(f => f.ArtifactPath.EndsWith("CreateInvoice.cs")).ShouldBeTrue();

    [Fact] void should_generate_invoice_overview_files() =>
        _generatedFiles.Any(f => f.ArtifactPath.EndsWith("InvoiceOverview.cs")).ShouldBeTrue();

    [Fact] void should_generate_automation_file() =>
        _generatedFiles.Any(f => f.ArtifactPath.EndsWith("SendReminder.cs")).ShouldBeTrue();

    [Fact] void should_generate_record_payment_slice_file() =>
        _generatedFiles.Any(f => f.ArtifactPath.EndsWith("RecordPayment.cs")).ShouldBeTrue();

    // ── Notifications module ───────────────────────────────────────────────────

    [Fact] void should_generate_notification_module_concept_file() =>
        _generatedFiles.Any(f => f.ArtifactPath.Contains("Notifications") && f.ArtifactPath.EndsWith("NotificationId.cs")).ShouldBeTrue();

    [Fact] void should_generate_push_feature_concept_files() =>
        _generatedFiles.Any(f => f.ArtifactPath.Contains("Push") && f.ArtifactPath.EndsWith("DeviceToken.cs"))
            .ShouldBeTrue();

    [Fact] void should_generate_email_translator_file() =>
        _generatedFiles.Any(f => f.ArtifactPath.EndsWith("IngestDelivery.cs")).ShouldBeTrue();

    [Fact] void should_not_generate_email_translator_external_event() =>
        _generatedFiles.Any(f => f.ArtifactPath.EndsWith("ExternalEmailDelivered.cs")).ShouldBeFalse();

    [Fact] void should_generate_delivery_log_file() =>
        _generatedFiles.Any(f => f.ArtifactPath.EndsWith("DeliveryLog.cs")).ShouldBeTrue();

    [Fact] void should_generate_send_push_file() =>
        _generatedFiles.Any(f => f.ArtifactPath.EndsWith("SendPush.cs")).ShouldBeTrue();

    // ── Cross-cutting: concept-as-property-type ────────────────────────────────

    [Fact] void should_use_concept_types_in_event_properties() =>
        _generatedFiles
            .First(f => f.ArtifactPath.EndsWith("RegisterPatient.cs"))
            .Content.ShouldContain("FullName Name");

    [Fact] void should_use_concept_types_in_command_properties() =>
        _generatedFiles
            .First(f => f.ArtifactPath.EndsWith("RegisterPatient.cs"))
            .Content.ShouldContain("FullName Name");

    [Fact] void should_use_concept_types_in_read_model_properties() =>
        _generatedFiles
            .First(f => f.ArtifactPath.EndsWith("PatientView.cs"))
            .Content.ShouldContain("FullName Name");

    [Fact] void should_use_module_level_concept_in_sub_feature_events() =>
        _generatedFiles
            .First(f => f.ArtifactPath.EndsWith("ScheduleAppointment.cs"))
            .Content.ShouldContain("PatientId PatientId");

    [Fact] void should_use_feature_level_concept_in_sub_feature_events() =>
        _generatedFiles
            .First(f => f.ArtifactPath.EndsWith("ScheduleAppointment.cs"))
            .Content.ShouldContain("AppointmentId AppointmentId");

    [Fact] void should_use_billing_concept_in_payment_event() =>
        _generatedFiles
            .First(f => f.ArtifactPath.EndsWith("RecordPayment.cs"))
            .Content.ShouldContain("Amount PaidAmount");

    [Fact] void should_add_concept_using_for_cross_namespace_reference() =>
        _generatedFiles
            .First(f => f.ArtifactPath.EndsWith("SendPush.cs"))
            .Content.ShouldContain("using Notifications;");

    // ── Compilation ────────────────────────────────────────────────────────────

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
