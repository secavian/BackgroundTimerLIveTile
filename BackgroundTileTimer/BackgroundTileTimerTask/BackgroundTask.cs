using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace BackgroundTileTimerTask
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var backgroundTaskDeferral = taskInstance.GetDeferral();
            try { UpdateTile(); }
            catch (Exception ex) { Debug.WriteLine(ex); }
            finally { backgroundTaskDeferral.Complete(); }
        }

        private void UpdateTile()
        {
            var now = DateTime.Now.ToString("HH:mm:ss");
            var xml =
                "<tile>" +
                "  <visual>" +
                "    <binding template='TileSmall'>" +
                "      <text>" + now + "</text>" +
                "    </binding>" +
                "    <binding template='TileMedium'>" +
                "      <text>" + now + "</text>" +
                "    </binding>" +
                "    <binding template='TileWide'>" +
                "      <text>" + now + "</text>" +
                "    </binding>" +
                "    <binding template='TileLarge'>" +
                "      <text>" + now + "</text>" +
                "    </binding>" +
                "  </visual>" +
                "</tile>";

            var tileDom = new XmlDocument();
            tileDom.LoadXml(xml);
            var tile = new TileNotification(tileDom);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tile);
        }
    }
}
