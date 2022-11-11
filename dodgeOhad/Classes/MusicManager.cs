using System;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.Storage.Streams;


namespace dodgeOhad
{
    public class MusicManager
    {
        private MediaElement _introMusic;
        private MediaElement _gameMusic;
        private MediaElement _collisionSoundElement;
        private MediaElement _bartHitSound;
        private MediaElement _bartLaugh;
        private MediaElement _gameOverMusic;
        private MediaElement _bartWinMusic;
        private MediaElement _bartMan;

        public MusicManager()
        {
            _gameMusic = new MediaElement();
            _introMusic = new MediaElement();
            _collisionSoundElement = new MediaElement();
            _bartHitSound = new MediaElement();
            _bartLaugh = new MediaElement();
            _gameOverMusic = new MediaElement();
            _bartWinMusic = new MediaElement();
            _bartMan = new MediaElement();
        }

        public async void PlayBackgroundMusic()
        {
            StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("sounds");
            StorageFile file = await folder.GetFileAsync("gameMusic.mp3");
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            _gameMusic.SetSource(stream, "");
            _gameMusic.Play();
        }

        public async void PlayCollisionSound()
        {
            StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("sounds");
            StorageFile file = await folder.GetFileAsync("boom1.mp3");
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            _collisionSoundElement.SetSource(stream, "");
            _collisionSoundElement.Play();
        }

        public async void PlayIntroMusic()
        {
            StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("sounds");
            StorageFile file = await folder.GetFileAsync("introMusic.mp3");
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            _introMusic.SetSource(stream, "");
            _introMusic.Play();
        }

        public async void PlayBartHitSound()
        {
            StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("sounds");
            StorageFile file = await folder.GetFileAsync("ayKaramba.mp3");
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            _bartHitSound.SetSource(stream, "");
            _bartHitSound.Play();
        }

        public async void PlayGameOverMusic()
        {
            StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("sounds");
            StorageFile file = await folder.GetFileAsync("gameOverMusic.mp3");
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            _gameOverMusic.SetSource(stream, "");
            _gameOverMusic.Play();
        }

        public async void PlayBartWinMusic()
        {
            StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("sounds");
            StorageFile file = await folder.GetFileAsync("winningMusic.mp3");
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            _bartWinMusic.SetSource(stream, "");
            _bartWinMusic.Play();
        }

        public async void BartLaugh()
        {
            StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("sounds");
            StorageFile file = await folder.GetFileAsync("BartLaugh.mp3");
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            _bartLaugh.SetSource(stream, "");
            _bartLaugh.Play();
        }

        public async void PlayBartMan()
        {
            StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("sounds");
            StorageFile file = await folder.GetFileAsync("BartMan.mp3");
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            _bartMan.SetSource(stream, "");
            _bartMan.Play();
        }

        internal void StopIntroMusic()
        {
            _introMusic.Stop();
        }

        internal void StopBgMusic()
        {
            _gameMusic.Stop();
        }

        internal void PauseBgMusic()
        {
            _gameMusic.Pause();
        }

        internal void ResmueBgMusic()
        {
            _gameMusic.Play();
        }

        internal void StopEnindgMusic()
        {
            _bartWinMusic.Stop();
            _gameOverMusic.Stop();
        }
    }
}
