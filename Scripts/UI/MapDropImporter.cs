using Quadrecep.Map;

namespace Quadrecep.UI
{
    public class MapDropImporter : FileDropHandler
    {
        public override void OnFileDrop(string[] files, int screen)
        {
            foreach (var file in files) MapSetImporter.ImportMapSet(file);
            RefreshSongSelectSlider();
        }

        private void RefreshSongSelectSlider()
        {
            GetNode<SongSelectSlider>("../SongSelectSlider").RefreshElements();
        }
    }
}