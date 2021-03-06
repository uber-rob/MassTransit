﻿// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.AzureServiceBusTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_message_to_a_session :
        AzureServiceBusTestFixture
    {
        [Test]
        public async void Should_have_a_redelivery_flag_of_false()
        {
            var context = await _handler;

            Assert.IsFalse(context.ReceiveContext.Redelivered);
        }

        [Test]
        public async void Should_succeed()
        {
            await _handler;
        }

        public Sending_a_message_to_a_session()
            : base("input_queue_session")
        {
        }

        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureInputQueueEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.RequiresSession = true;

            _handler = Handled<PingMessage>(configurator);
        }

        [TestFixtureSetUp]
        public void Setup()
        {
            Await(() =>
            {
                var message = new PingMessage();
                return InputQueueSendEndpoint.Send(message, context =>
                {
                    context.SetSessionId(message.CorrelationId.ToString());
                });
            });
        }
    }
}