// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.Chronicle;
using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_processing;

public class with_chronicle_and_internal_events : given.a_module_with_a_slice_producing_files
{
    IChronicleRegistration _chronicle;
    VerticalSlicesEngine _engine;

    void Establish()
    {
        _chronicle = Substitute.For<IChronicleRegistration>();
        _chronicleResolver.Resolve().Returns(_chronicle);
        _engine = new VerticalSlicesEngine(_codeGenerator, _logger, _outputResolver, _chronicleResolver);
    }

    async Task Because() => await _engine.Process(_modules);

    [Fact] void should_register_event_types_with_chronicle() => _chronicle.Received(1).RegisterEventTypes(Arg.Any<IEnumerable<EventTypeDescriptor>>(), Arg.Any<CancellationToken>());
    [Fact] void should_register_one_event_type() => _chronicle.Received(1).RegisterEventTypes(Arg.Is<IEnumerable<EventTypeDescriptor>>(e => e.Count() == 1), Arg.Any<CancellationToken>());
    [Fact] void should_register_event_type_with_correct_name() => _chronicle.Received(1).RegisterEventTypes(Arg.Is<IEnumerable<EventTypeDescriptor>>(e => e.First().Name == "OrderPlaced"), Arg.Any<CancellationToken>());
}
