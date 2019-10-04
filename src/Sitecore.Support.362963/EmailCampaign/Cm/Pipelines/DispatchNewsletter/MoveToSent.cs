namespace Sitecore.Support.EmailCampaign.Cm.Pipelines.DispatchNewsletter
{
    using System;
    using Sitecore.Diagnostics;
    using Sitecore.EmailCampaign.Cm.Pipelines.DispatchNewsletter;
    using Sitecore.EmailCampaign.Model.Message;
    using Sitecore.ExM.Framework.Diagnostics;
    using Sitecore.SecurityModel;
    using Sitecore.Modules.EmailCampaign.Core.Data;
    public class MoveToSent
    {
        private readonly ILogger _logger;
        private readonly EcmDataProvider _dataProvider;

        public MoveToSent(ILogger logger, EcmDataProvider dataProvider)
        {
            Assert.ArgumentNotNull(logger, "logger");
            _logger = logger;
            _dataProvider = dataProvider;
        }

        public void Process(DispatchNewsletterArgs args)
        {
            if (args.IsTestSend || !args.RequireFinalMovement || args.SendingAborted || args.Message.MessageType == MessageType.Automated)
            {
                return;
            }

            if (args.Message.Emulation)
            {
                _logger.LogDebug("Message not moved to Sent state as emulation mode is enabled.");
                using (new SecurityDisabler())
                {
                    args.Message.Source.State = MessageState.Draft;
                }
                return;
            }

            using (new SecurityDisabler())
            {
                args.Message.Source.ReleaseRelatedItems();
                args.Message.Source.State = MessageState.Sent;
                args.Message.Source.EndTime = DateTime.UtcNow;
                _dataProvider.SaveCampaign(args.Message, null);
            }
        }
    }
}