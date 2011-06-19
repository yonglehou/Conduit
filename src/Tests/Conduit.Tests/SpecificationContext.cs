using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Conduit.Tests
{
    [TestClass]
    public abstract class SpecificationContext
    {
        [TestInitialize]
        public void Init()
        {
            this.Given();
            this.When();
        }

        public virtual void Given() { }
        public virtual void When() { }
        public virtual void Cleanup() { }
    }
}
