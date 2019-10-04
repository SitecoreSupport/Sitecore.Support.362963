namespace Sitecore.Support.EmailCampaign.Cm.Pipelines.DispatchNewsletter
{
    using Sitecore.Diagnostics;
    using Sitecore.EmailCampaign.Cm.Pipelines.DispatchNewsletter;
    using Sitecore.EmailCampaign.Model.Message;
    using Sitecore.EmailCampaign.Model.Web;
    using Sitecore.ExM.Framework.Diagnostics;
    using Sitecore.SecurityModel;
    public class MoveToQueuing
    {
        private readonly ILogger _logger;

        public MoveToQueuing(ILogger logger)
        {
            Assert.ArgumentNotNull(logger, "logger");
            _logger = logger;
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
            }
        }
    }
}