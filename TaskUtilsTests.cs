using NUnit.Framework;

[TestFixture, Timeout(150)]
public class TaskUtilsTests
{
    [Test]
    public void WaitUntil_GivenAnAlreadyMetCondition_CompletesImmediately()
    {
        var sut = TaskUtils.WaitUntil(() => true);
        Assert.That(sut, Has.Property(nameof(Task.IsCompleted)).True);
    }

    [Test]
    public async Task WaitUntil_CompletesWhenConditionIsMet()
    {
        var condition = false;
        var sut = TaskUtils.WaitUntil(() => condition);
        Assume.That(sut, Has.Property(nameof(Task.IsCompleted)).False);

        condition = true;
        await sut;

        Assert.That(sut, Has.Property(nameof(Task.IsCompleted)).True);
    }

    [Test]
    public void WaitUntil_DoesNotCompleteIfConditionIsNotMet()
    {
        var sut = TaskUtils.WaitUntil(() => false);

        Assert.That(sut, Has.Property(nameof(Task.IsCompleted)).Not.True.After(75).MilliSeconds);
    }

    [Test]
    public void WaitUntil_OnCanceled_ThrowsTaskCanceledException()
    {
        using var cts = new CancellationTokenSource();
        var sut = TaskUtils.WaitUntil(() => false, cts.Token);

        cts.Cancel();

        Assert.CatchAsync<TaskCanceledException>(async () => await sut);
    }
}
