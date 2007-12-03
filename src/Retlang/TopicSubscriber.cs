using System;

namespace Retlang
{
    public interface ISubscriber
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="consumed">set to true ONLY if the subscriber consumes the event. Do NOT set to false since events are invoked using a multicast event</param>
        void Receive(ITransferEnvelope envelope, ref bool consumed);
    }

    public class TopicSubscriber<T> : ISubscriber
    {
        private readonly ITopicMatcher _topic;
        private readonly OnMessage<T> _onMessage;
        private readonly Type _type;

        public TopicSubscriber(ITopicMatcher topic, OnMessage<T> onMessage)
        {
            _topic = topic;
            _onMessage = onMessage;
            _type = typeof (T);
        }

        public ITopicMatcher Topic
        {
            get { return _topic; }
        }

        public Type MessageType
        {
            get { return _type; }
        }

        public void Receive(ITransferEnvelope envelope, ref bool consumed)
        {
            if (_topic.Matches(envelope.Header.Topic))
            {
                if (MessageType.IsAssignableFrom(envelope.MessageType))
                {
                    T typedMsg = (T) envelope.ResolveMessage();
                    _onMessage(envelope.Header, typedMsg);
                    consumed = true;
                }
            }
        }
    }
}