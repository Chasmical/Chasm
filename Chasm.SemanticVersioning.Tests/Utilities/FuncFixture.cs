using System;
using Chasm.Utilities;
using JetBrains.Annotations;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public abstract class FuncFixture<T> : Fixture, IFuncFixture<T>
    {
        public Type? ExceptionType { get; private set; }
        public string? ExceptionMessage { get; private set; }
        protected virtual Type DefaultExceptionType => typeof(ArgumentException);

        public void Throws<TException>() where TException : Exception
            => Throws(typeof(TException), null);
        public void Throws<TException>(string? exceptionMessage) where TException : Exception
            => Throws(typeof(TException), exceptionMessage);
        public void Throws(string? exceptionMessage)
            => Throws(DefaultExceptionType, exceptionMessage);
        public void Throws(Type? exceptionType, string? exceptionMessage)
        {
            MarkAsComplete(false);
            ExceptionType = exceptionType;
            ExceptionMessage = exceptionMessage;
        }

        public abstract void AssertResult(T? result);
        public virtual void AssertException(Exception? exception)
        {
            Assert.IsType(ExceptionType!, exception);
            if (ExceptionMessage is not null)
                Assert.StartsWith(ExceptionMessage, exception.Message);
        }

        public void Test([InstantHandle] Func<T> func)
        {
            Exception? exception = Util.Catch(func, out T? result);
            Assert.Equal(IsValid, exception is null);
            if (exception is null) AssertResult(result);
            else AssertException(exception);
        }
        public void Test(bool success, T? result)
        {
            Assert.Equal(IsValid, success);
            if (success) AssertResult(result);
        }

        protected TExtender Extend<TExtender>() where TExtender : IFixtureExtender<IFuncFixture<T>>, new()
        {
            TExtender extender = new TExtender();
            extender.SetPrototype(this);
            return extender;
        }

    }
    public interface IFuncFixture<in T>
    {
        void Test([InstantHandle] Func<T> func);
        void Test(bool success, T? result);
    }
}
