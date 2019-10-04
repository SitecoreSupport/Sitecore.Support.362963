namespace Sitecore.Support.EmailCampaign.Cm.Pipelines.DispatchNewsletter
{
    using Sitecore.Diagnostics;
    using Sitecore.EmailCampaign.Cm.Pipelines.DispatchNewsletter;
    using Sitecore.EmailCampaign.Model.Message;
    using Sitecore.EmailCampaign.Model.Web;
    using Sitecore.ExM.Framework.Diagnostics;
    using Sitecore.SecurityModel;
    using Sitecore.Modules.EmailCampaign.Core.Data;
    public class MoveToQueuing
    {
        private readonly ILogger _logger;
        private readonly EcmDataProvider _dataProvider;

        public MoveToQueuing(ILogger logger, EcmDataProvider dataProvider)
        {
            Assert.ArgumentNotNull(logger, "logger");
            _logger = logger;
            _dataProvider = dataProvider;
        }

        public void Process(DispatchNewsletterArgs args)
        {
            if (args.IsTestSend || !args.RequireInitialMovement)
            {
                return;
            }

            using (new SecurityDisabler())
            {
                if (args.Message.State == MessageState.Queuing || args.Message.State == MessageState.Active)
                {
                    return;
                }

                args.Message.Source.State = MessageState.Queuing;

                if (!args.Message.Source.LockRelatedItems())
                {
                    args.AbortSending(EcmTexts.Localize(EcmTexts.UnableGetTargetItem), true, _logger);
                }

                _dataProvider.SaveCampaign(args.Message, null);
            }
        }
    }
}