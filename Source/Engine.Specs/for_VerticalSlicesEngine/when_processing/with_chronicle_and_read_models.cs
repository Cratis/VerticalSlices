// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.VerticalSlices.Chronicle;
using Cratis.VerticalSlices.CodeGeneration.Descriptors;

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_processing;

public class with_chronicle_and_read_models : given.a_module_with_a_slice_producing_files
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

    [Fact] void should_register_projections_with_chronicle() => _chronicle.Received(1).RegisterProjections(Arg.Any<IEnumerable<ReadModelDescriptor>>(), Arg.Any<CancellationToken>());
    [Fact] void should_register_read_model_types_with_chronicle() => _chronicle.Received(1).RegisterReadModelTypes(Arg.Any<IEnumerable<ReadModelDescriptor>>(), Arg.Any<CancellationToken>());
    [Fact] void should_register_one_read_model() => _chronicle.Received(1).RegisterProjections(Arg.Is<IEnumerable<ReadModelDescriptor>>(r => r.Count() == 1), Arg.Any<CancellationToken>());
    [Fact] void should_register_read_model_with_correct_name() => _chronicle.Received(1).RegisterProjections(Arg.Is<IEnumerable<ReadModelDescriptor>>(r => r.First().Name == "OrderList"), Arg.Any<CancellationToken>());
}
