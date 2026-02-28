// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.Integration.when_processing_a_complete_module;

/// <summary>
/// Comprehensive end-to-end scenario: a HealthCare module that contains all four slice types
/// (StateChange, StateView, Automation, Translator), module-level and feature-level concepts
/// with validation rules, and a feature with nested sub-features is fed to the engine.
/// All generated C# files are written to disk and compiled in a single <c>dotnet build</c>
/// invocation to verify the full code-generation pipeline produces valid, co-compilable output.
/// </summary>
public class with_all_slice_types_in_a_realistic_domain : given.a_real_engine
{
    IEnumerable<Module> _modules;

    void Establish()
    {
        // ── Concepts ────────────────────────────────────────────────────────────
        var patientId = new Concept("PatientId", "Guid", "Uniquely identifies a patient", []);
        var doctorId = new Concept("DoctorId", "Guid", "Uniquely identifies a doctor", []);

        var patientName = new Concept(
            "PatientName",
            "string",
            "The full name of a patient",
            [
                new ConceptValidationRule(ConceptValidationRuleType.NotEmpty),
                new ConceptValidationRule(ConceptValidationRuleType.MinimumLength, "2"),
                new ConceptValidationRule(ConceptValidationRuleType.MaximumLength, "200")
            ]);

        // ── Events ──────────────────────────────────────────────────────────────
        var patientRegistered = new EventType(
            "PatientRegistered",
            "A patient was registered",
            [
                new Property("PatientId", "string"),
                new Property("FullName", "string"),
                new Property("DateOfBirth", "DateTimeOffset")
            ]);

        var appointmentScheduled = new EventType(
            "AppointmentScheduled",
            "An appointment was scheduled",
            [
                new Property("AppointmentId", "string"),
                new Property("PatientId", "string"),
                new Property("DoctorId", "string"),
                new Property("ScheduledAt", "DateTimeOffset")
            ]);

        var appointmentCancelled = new EventType(
            "AppointmentCancelled",
            "An appointment was cancelled",
            [
                new Property("AppointmentId", "string")
            ]);

        var externalCalendarEvent = new EventType(
            "ExternalCalendarEvent",
            "An event from the external calendar system",
            [
                new Property("ExternalRef", "string"),
                new Property("AppointmentId", "string")
            ],
            EventKind.External);

        var calendarEventSynced = new EventType(
            "CalendarEventSynced",
            "An external calendar event was synced internally",
            [
                new Property("AppointmentId", "string"),
                new Property("ExternalRef", "string")
            ],
            EventKind.Internal);

        // ── Patients feature ─────────────────────────────────────────────────────
        var registerPatientCommand = new Command(
            "RegisterPatient",
            "Registers a new patient",
            [
                new Property("FullName", "string"),
                new Property("DateOfBirth", "DateTimeOffset")
            ],
            "PatientId");

        var registrationSlice = new VerticalSlice(
            "RegisterPatient",
            VerticalSliceType.StateChange,
            null,
            null,
            [registerPatientCommand],
            [],
            [patientRegistered]);

        var patientIdMappings = new[] { new EventPropertyMapping("PatientRegistered", EventPropertyMappingKind.Set, "PatientId") };
        var fullNameMappings = new[] { new EventPropertyMapping("PatientRegistered", EventPropertyMappingKind.Set, "FullName") };
        var registeredAtMappings = new[] { new EventPropertyMapping("PatientRegistered", EventPropertyMappingKind.SetFromContext, "Occurred") };

        var patientSummary = new ReadModel(
            "PatientSummary",
            "A summary view of a registered patient",
            [
                new ReadModelProperty("PatientId", "string", patientIdMappings),
                new ReadModelProperty("FullName", "string", fullNameMappings),
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

        var patientsFeature = new Feature(
            "Patients",
            [patientName],
            [],
            [registrationSlice, patientViewSlice]);

        // ── Appointments feature — nested sub-features ──────────────────────────
        var scheduleCommand = new Command(
            "ScheduleAppointment",
            "Schedules a new appointment for a patient",
            [
                new Property("PatientId", "string"),
                new Property("DoctorId", "string"),
                new Property("ScheduledAt", "DateTimeOffset")
            ],
            "AppointmentId");

        var cancelCommand = new Command(
            "CancelAppointment",
            "Cancels an existing appointment",
            [],
            "AppointmentId");

        var schedulingSlice = new VerticalSlice(
            "ScheduleAppointment",
            VerticalSliceType.StateChange,
            null,
            null,
            [scheduleCommand, cancelCommand],
            [],
            [appointmentScheduled, appointmentCancelled]);

        var apptIdMappings = new[] { new EventPropertyMapping("AppointmentScheduled", EventPropertyMappingKind.Set, "AppointmentId") };
        var apptPatientMappings = new[] { new EventPropertyMapping("AppointmentScheduled", EventPropertyMappingKind.Set, "PatientId") };
        var scheduledCountMappings = new[] { new EventPropertyMapping("AppointmentScheduled", EventPropertyMappingKind.Count) };
        var cancelledCountMappings = new[] { new EventPropertyMapping("AppointmentCancelled", EventPropertyMappingKind.Decrement) };

        var upcomingAppointment = new ReadModel(
            "UpcomingAppointment",
            "An appointment that has not yet occurred",
            [
                new ReadModelProperty("AppointmentId", "string", apptIdMappings),
                new ReadModelProperty("PatientId", "string", apptPatientMappings),
                new ReadModelProperty("TotalScheduled", "int", scheduledCountMappings),
                new ReadModelProperty("TotalCancelled", "int", cancelledCountMappings)
            ]);

        var sendReminderCommand = new Command(
            "SendAppointmentReminder",
            "Sends a reminder for an upcoming appointment",
            [],
            "AppointmentId");

        var remindersSlice = new VerticalSlice(
            "AppointmentReminders",
            VerticalSliceType.Automation,
            null,
            null,
            [sendReminderCommand],
            [upcomingAppointment],
            []);

        var translatorSlice = new VerticalSlice(
            "SyncCalendar",
            VerticalSliceType.Translator,
            null,
            null,
            [],
            [],
            [externalCalendarEvent, calendarEventSynced]);

        var schedulingSubFeature = new Feature("Scheduling", [], [], [schedulingSlice, remindersSlice]);
        var externalSyncSubFeature = new Feature("ExternalSync", [], [], [translatorSlice]);

        var appointmentsFeature = new Feature(
            "Appointments",
            [],
            [schedulingSubFeature, externalSyncSubFeature],
            []);

        _modules = [new Module("HealthCare", [patientId, doctorId], [patientsFeature, appointmentsFeature])];
    }

    async Task Because()
    {
        await _engine.Process(_modules, _output);
        _generatedFiles = _engine.Preview(_modules);
        AddGlobalUsingsFromGeneratedFiles();
        _buildExitCode = await RunDotnet("build");
    }

    [Fact] void should_generate_module_level_concept_files() =>
        new[] { "PatientId.cs", "DoctorId.cs" }
            .All(n => _generatedFiles.Any(f => f.RelativePath.EndsWith(n)))
            .ShouldBeTrue();

    [Fact] void should_generate_feature_level_concept_with_validator() =>
        new[] { "PatientName.cs", "PatientNameValidator.cs" }
            .All(n => _generatedFiles.Any(f => f.RelativePath.EndsWith(n)))
            .ShouldBeTrue();

    [Fact] void should_generate_state_change_files() =>
        new[] { "PatientRegistered.cs", "RegisterPatient.cs", "AppointmentScheduled.cs", "AppointmentCancelled.cs", "ScheduleAppointment.cs", "CancelAppointment.cs" }
            .All(n => _generatedFiles.Any(f => f.RelativePath.EndsWith(n)))
            .ShouldBeTrue();

    [Fact] void should_generate_state_view_files() =>
        new[] { "PatientSummary.cs", "AllPatientSummarys.cs" }
            .All(n => _generatedFiles.Any(f => f.RelativePath.EndsWith(n)))
            .ShouldBeTrue();

    [Fact] void should_generate_automation_files() =>
        new[] { "UpcomingAppointment.cs", "AllUpcomingAppointments.cs", "SendAppointmentReminder.cs" }
            .All(n => _generatedFiles.Any(f => f.RelativePath.EndsWith(n)))
            .ShouldBeTrue();

    [Fact] void should_generate_internal_translator_event_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("CalendarEventSynced.cs")).ShouldBeTrue();

    [Fact] void should_not_generate_external_event_file() =>
        _generatedFiles.Any(f => f.RelativePath.EndsWith("ExternalCalendarEvent.cs")).ShouldBeFalse();

    [Fact] void should_compile_successfully() => _buildExitCode.ShouldEqual(0);
}
