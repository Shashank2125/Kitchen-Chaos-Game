using System;

public interface IProgress
{

    event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;


    public class OnProgressChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }
}

