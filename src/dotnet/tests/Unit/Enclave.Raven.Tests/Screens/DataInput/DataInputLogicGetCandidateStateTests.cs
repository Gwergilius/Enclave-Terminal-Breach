using Enclave.Common.Test.Core;
using Enclave.Echelon.Core.Models;
using Enclave.Raven.Screens.DataInput;

namespace Enclave.Raven.Tests.Screens.DataInput;

[UnitTest, TestOf(nameof(DataInputLogic))]
public class DataInputLogicGetCandidateStateTests
{
    // --- CandidateCountLine format ---

    [Fact]
    public void GetCandidateState_Always_FormatsCandidateCountLine()
    {
        var session = Mock.Of<IGameSession>();
        session.AsMock().SetupGet(s => s.Count).Returns(3);
        session.AsMock().SetupGet(s => s.WordLength).Returns((int?)null);

        var (countLine, _) = DataInputLogic.GetCandidateState(session);

        countLine.ShouldBe("3 candidate(s):");
    }

    // --- CandidateListText: null conditions ---

    [Fact]
    public void GetCandidateState_WhenCountIsZero_ReturnsNullListText()
    {
        var session = Mock.Of<IGameSession>();
        session.AsMock().SetupGet(s => s.Count).Returns(0);
        session.AsMock().SetupGet(s => s.WordLength).Returns(5);

        var (_, listText) = DataInputLogic.GetCandidateState(session);

        listText.ShouldBeNull();
    }

    [Fact]
    public void GetCandidateState_WhenWordLengthIsNull_ReturnsNullListText()
    {
        var session = Mock.Of<IGameSession>();
        session.AsMock().SetupGet(s => s.Count).Returns(3);
        session.AsMock().SetupGet(s => s.WordLength).Returns((int?)null);

        var (_, listText) = DataInputLogic.GetCandidateState(session);

        listText.ShouldBeNull();
    }

}
