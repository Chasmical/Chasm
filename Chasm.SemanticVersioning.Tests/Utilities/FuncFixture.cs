using System;
using Chasm.Utilities;
using JetBrains.Annotations;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("xUnit", "xUnit3001", Justification = "Class already has an implicit constructor.")]
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
            Assert.Multiple(
                () => Assert.IsType(ExceptionType!, exception),
                () =>
                {
                    if (ExceptionMessage is not null && exception is not null)
                        Assert.StartsWith(ExceptionMessage, exception.Message);
                }
            );
        }

        public void Test([InstantHandle] Func<T> func)
        {
            Exception? exception = Util.Catch(func, out T? result);
            if (IsValid)
            {
                if (exception is not null) Assert.Fail("Expected the test to succeed, but an exception was raised:\n" + exception);
            }
            else
            {
                if (exception is null) Assert.Fail("Expected the test to fail, but a result was returned:\n" + result);
            }
            if (exception is null) AssertResult(result);
            else AssertException(exception);
        }
        public void Test(bool success, T? result)
        {
            if (IsValid)
            {
                if (!success) Assert.Fail("Expected the test to succeed, but it failed.");
                AssertResult(result);
            }
            else
            {
                if (success) Assert.Fail("Expected the test to fail, but a result was returned:\n" + result);
            }
        }

    }
    public interface IFuncFixture<in T>
    {
        void Test([InstantHandle] Func<T> func);
        void Test(bool success, T? result);
    }
}
