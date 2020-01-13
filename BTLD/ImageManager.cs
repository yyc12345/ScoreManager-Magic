using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace BTLD {

    public abstract class ImageManager {

        protected Dictionary<string, BitmapImage> innerStorage = new Dictionary<string, BitmapImage>();

        public BitmapImage this[string index] {
            get {
                if (!innerStorage.ContainsKey(index)) LoadImage(index);
                return innerStorage[index];
            }
        }

        protected virtual void LoadImage(string name) => throw new NotImplementedException();

    }

    public class UserAvatarManager : ImageManager {
        public UserAvatarManager() {
            innerStorage.Add("", new BitmapImage(new Uri("pack://application:,,,/BTLD;component/Resources/DefaultUser.jpg")));
        }

        protected override void LoadImage(string name) {
            innerStorage.Add(name, new BitmapImage(new Uri(SMMLib.Utilities.Information.WorkPath.Enter("user").Enter(name + ".jpg").Path, UriKind.Absolute)));
        }
    }

    public class MapPreviewManager : ImageManager {
        public MapPreviewManager() {
            innerStorage.Add("", new BitmapImage(new Uri("pack://application:,,,/BTLD;component/Resources/DefaultMap.jpg")));
        }

        protected override void LoadImage(string name) {
            innerStorage.Add(name, new BitmapImage(new Uri(SMMLib.Utilities.Information.WorkPath.Enter("map").Enter(name + ".jpg").Path, UriKind.Absolute)));
        }
    }

}
