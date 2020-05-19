using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Links.UrlBuilders;
using Sitecore.Resources.Media;
using Sitecore.Web;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using System;
using System.Web.UI.WebControls;
using ImageField = Sitecore.Data.Fields.ImageField;

namespace ImageFocalPointPicker.Dialogs
{
    public class ImageFocalPointPickerDialog : DialogForm
    {
        private const string Separator = "|";

        private readonly Database _masterDb = Factory.GetDatabase("master");

        public Sitecore.Web.UI.HtmlControls.Image ImageFrame;

        public Sitecore.Web.UI.HtmlControls.Edit TextBoxCoordinate;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (ImageFrame == null) return;

            TextBoxCoordinate.Value = WebUtil.GetQueryString("value");
            string containerId = WebUtil.GetQueryString("containerId");
            string fieldId = WebUtil.GetQueryString("fieldId");
            Item currentItem = _masterDb.Items.GetItem(containerId);

            ImageField imageField = currentItem.Fields[fieldId];

            if (imageField == null || string.IsNullOrWhiteSpace(imageField.Value))
            {
                ImageFrame.Alt = "No image available";
                ImageFrame.Src = "#";
                return;
            }

            string imageSrc = MediaManager.GetMediaUrl(
                _masterDb.Items.GetItem(imageField.MediaID),
                new MediaUrlBuilderOptions
                {
                    Database = _masterDb,
                    DisableMediaCache = true,
                    DisableBrowserCache = true,
                    AllowStretch = false
                });

            if (!string.IsNullOrWhiteSpace(imageSrc))
            {
                imageSrc += "&usecustomfunctions=1&centercrop=1";
            }

            ImageFrame.Src = imageSrc;
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            SheerResponse.SetDialogValue(this.TextBoxCoordinate.Value);
            base.OnOK(sender, args);
        }
    }
}