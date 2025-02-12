#if RUNTIME
global using TestAttribute = BeamOs.Tests.Common.TestAttribute;
global using MethodDataSourceAttribute = BeamOs.Tests.Common.MethodDataSourceAttribute;
global using ParallelGroupAttribute = BeamOs.Tests.Common.ParallelGroupAttribute;
#else
global using TestAttribute = TUnit.Core.TestAttribute;
global using MethodDataSourceAttribute = TUnit.Core.MethodDataSourceAttribute;
global using ParallelGroupAttribute = TUnit.Core.ParallelGroupAttribute;
#endif
