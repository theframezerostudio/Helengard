public struct AudioHandle
{
    private AudioChannel channel;
    private AudioBox box;
    private readonly int generationId;

    public AudioHandle(AudioChannel channel, AudioBox box, int generationId)
    {
        this.channel = channel;
        this.box = box;
        this.generationId = generationId;
    }

    public void Stop()
    {
        if (IsValid())
        {
            channel.Stop(box);
        }

        channel = null;
        box = null;
    }

    public readonly bool IsValid()
    {
        return channel != null && box != null && box.GenerationId == generationId;
    }

    public readonly bool IsPlaying()
    {
        if (!IsValid())
            return false;

        return box.IsPlaying;
    }
}