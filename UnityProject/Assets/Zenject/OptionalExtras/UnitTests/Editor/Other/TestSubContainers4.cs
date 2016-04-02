using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class TestSubContainers4 : TestWithContainer
    {
        readonly Dictionary<object, DiContainer> _subContainers = new Dictionary<object, DiContainer>();

        [Test]
        public void RunTest()
        {
            SetupContainer();

            var view1 = Container.Resolve<RotorView>();

            Assert.IsEqual(view1.Controller.Model, view1.Model);

            var view2 = Container.Resolve<RotorView>();

            Assert.IsEqual(view2.Controller.Model, view2.Model);

            Assert.IsNotEqual(view2.Model, view1.Model);
            Assert.IsNotEqual(view2, view1);
        }

        void SetupContainer()
        {
            Container.Bind<RotorController>().ToMethodSelf(SubContainerResolve<RotorController>).AsTransient()
                .WhenInjectedInto<RotorView>();

            Container.Bind<RotorModel>().ToMethodSelf(SubContainerResolve<RotorModel>).AsTransient()
                .WhenInjectedInto<RotorView>();

            Container.Bind<RotorView>().ToSelf().AsTransient();
        }

        T SubContainerResolve<T>(InjectContext context)
        {
            Assert.IsNotNull(context.ObjectInstance);
            DiContainer subContainer;

            if (!_subContainers.TryGetValue(context.ObjectInstance, out subContainer))
            {
                subContainer = context.Container.CreateSubContainer();
                _subContainers.Add(context.ObjectInstance, subContainer);

                InstallViewBindings(subContainer);
            }

            return (T)subContainer.Resolve(context);
        }

        void InstallViewBindings(DiContainer subContainer)
        {
            subContainer.Bind<RotorController>().ToSelf().AsSingle();
            subContainer.Bind<RotorModel>().ToSelf().AsSingle();
        }

        public class RotorController
        {
            [Inject]
            public RotorModel Model;
        }

        public class RotorView
        {
            [Inject]
            public RotorController Controller;

            [Inject]
            public RotorModel Model;
        }

        public class RotorModel
        {
        }
    }
}
