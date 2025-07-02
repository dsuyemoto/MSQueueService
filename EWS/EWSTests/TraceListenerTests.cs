using NUnit.Framework;
using Moq;
using LoggerHelper;

namespace EWS.Tests
{
    [TestFixture()]
    public class TraceListenerTests
    {
        TraceListener _traceListener;
        IFileWrapper _mockFileWrapper;

        [SetUp()]
        public void TraceListenerSetup()
        {
            _mockFileWrapper = Mock.Of<IFileWrapper>();
            Mock.Get(_mockFileWrapper).Setup(f => f.AppendAllText(It.IsAny<string>(), It.IsAny<string>()));
            Mock.Get(_mockFileWrapper).Setup(f => f.ReadAllBytes(It.IsAny<string>())).Returns(new byte[1]);
            Mock.Get(_mockFileWrapper).Setup(f => f.WriteAllText(It.IsAny<string>(), It.IsAny<string>()));
            Mock.Get(_mockFileWrapper).Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(true);
            Mock.Get(_mockFileWrapper).Setup(f => f.CreateDirectory(It.IsAny<string>()));
        }

        [Test()]
        public void InstanceTest()
        {
            _traceListener = TraceListener.StartNewLog("testFolderPath",  _mockFileWrapper, new Logger());

            Assert.IsNotNull(_traceListener);
        }

        [Test()]
        public void Trace_AppendAllText_CalledOnceTest()
        {
            Mock.Get(_mockFileWrapper).Setup(f => f.Exists(It.IsAny<string>())).Returns(false);

            _traceListener = TraceListener.StartNewLog("testFolderPath", _mockFileWrapper, new Logger());
            _traceListener.Trace("traceType", "traceContent");

            Mock.Get(_mockFileWrapper).Verify(f => f.AppendAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test()]
        public void Trace_ReadAllBytes_CalledOnceTest()
        {
            Mock.Get(_mockFileWrapper).Setup(f => f.Exists(It.IsAny<string>())).Returns(true);

            _traceListener = TraceListener.StartNewLog("testFolderPath", _mockFileWrapper, new Logger());
            _traceListener.Trace("traceType", "traceContent");

            Mock.Get(_mockFileWrapper).Verify(f => f.ReadAllBytes(It.IsAny<string>()), Times.Once);
        }
    }
}