using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace dodgeOhad.Classes
{
    public static class FileController
    {
        private const string FILE_NAME = "simpsons_saved_game.js";

        public static async void SaveToFile(List<PlayerModel> itemsToSave)
        {
            string contentToSave = JsonConvert.SerializeObject(itemsToSave, Formatting.Indented);
            StorageFolder stroageFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await stroageFolder.CreateFileAsync(FILE_NAME, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(sampleFile, contentToSave);
        }

        public static async Task<List<PlayerModel>> LoadFromFile()
        {
            StorageFolder stroageFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile;
            try
            {
                sampleFile = await stroageFolder.GetFileAsync(FILE_NAME);
            }
            catch (Exception)
            {
                return null;
            }

            string jsonFromFile = await FileIO.ReadTextAsync(sampleFile);
            List<PlayerModel> playerModels = JsonConvert.DeserializeObject<List<PlayerModel>>(jsonFromFile);
            return playerModels;
        }
    }
}
