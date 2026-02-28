// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.VerticalSlices.for_VerticalSlicesEngine.when_processing;

public class with_invalid_slice : given.all_dependencies
{
    VerticalSlicesEngine _engine;
    IEnumerable<Module> _modules;
    Exception _exception;

    void Establish()
    {
        _engine = new VerticalSlicesEngine(_codeGenerator, _logger);
        var externalEvent = new EventType("ExternalEvent", "External", [], EventKind.External);
        var slice = new VerticalSlice("BadSlice", VerticalSliceType.StateChange, null, null, [], [], [externalEvent]);
        _modules = [new Module("Mod", [], [new Feature("Feat", [], [], [slice])])];
    }

    async Task Because()
    {
        try
        {
            await _engine.Process(_modules);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Fact] void should_throw_slice_validation_failed() => _exception.ShouldBeOfExactType<SliceValidationFailed>();
}
