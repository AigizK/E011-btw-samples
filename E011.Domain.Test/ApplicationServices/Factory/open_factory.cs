using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using E005_testing_use_cases;
using NUnit.Framework;

namespace E011.Domain.ApplicationServices.Factory
{
    public class open_factory : factory_syntax
    {
        [Test]
        public void correct_open_factory()
        {
            When(new OpenFactory(FactoryId.ForTest));
            Expect(new FactoryOpened(FactoryId.ForTest));
        }

        [Test]
        public void attempt_to_open_more_than_once()
        {
            Given(new FactoryOpened(FactoryId.ForTest));
            When(new OpenFactory(FactoryId.ForTest));
            Expect("factory-already-created");
        }
    }
}
