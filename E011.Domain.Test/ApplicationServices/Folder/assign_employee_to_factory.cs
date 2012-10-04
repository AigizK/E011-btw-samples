using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using E011;
using NUnit.Framework;

namespace E005_testing_use_cases.unittest
{
    public class assign_employee_to_factory : factory_syntax
    {
        [Test]
        public void empty_factory()
        {
            Given(new FactoryOpened(FactoryId.ForTest));
            When(new AssignEmployeeToFactory(FactoryId.ForTest,"fry"));
            Expect(new EmployeeAssignedToFactory(FactoryId.ForTest, "fry"));
        }
        [Test]
        public void fry_is_assigned_to_factory()
        {
            Given(new FactoryOpened(FactoryId.ForTest),
                    new EmployeeAssignedToFactory(FactoryId.ForTest,"fry"));
            When(new AssignEmployeeToFactory(FactoryId.ForTest, "fry"));
            Expect("more than 1 person");
        }
        [Test]
        public void bender_comes_to_empty_factory()
        {
            Given(new FactoryOpened(FactoryId.ForTest));
            When(new AssignEmployeeToFactory(FactoryId.ForTest, "bender"));
            Expect("bender-employee");
        }
    }
}
