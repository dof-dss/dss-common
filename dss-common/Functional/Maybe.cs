using System;
using System.Collections.Generic;
using System.Linq;

namespace dss_common.Functional
{
    public struct Maybe<T>
    {
        public bool Equals(Maybe<T> other)
        {
            return Equals(_values, other._values);
        }

        public override bool Equals(object obj)
        {
            return obj is Maybe<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (_values != null ? _values.GetHashCode() : 0);
        }

        readonly IEnumerable<T> _values;

        public static Maybe<T> Some(T value)
        {
            if (value == null)
            {
                throw new InvalidOperationException();
            }

            return new Maybe<T>(new[] { value });
        }

        public static Maybe<T> None => new Maybe<T>(new T[0]);

        Maybe(IEnumerable<T> values)
        {
            this._values = values;
        }

        public bool HasValue => _values != null && _values.Any();

        public T Value
        {
            get
            {
                if (!HasValue)
                {
                    throw new InvalidOperationException("Maybe does not have a value");
                }

                return _values.Single();
            }
        }

        public T ValueOrDefault(T @default)
        {
            if (!HasValue)
            {
                return @default;
            }
            return _values.Single();
        }

        public T ValueOrThrow(Exception e)
        {
            if (HasValue)
            {
                return Value;
            }
            throw e;
        }

        public U Match<U>(Func<T, U> some, Func<U> none)
        {
            return HasValue
                ? some(Value)
                : none();
        }

        public void Match(Action<T> some, Action none)
        {
            if (HasValue)
            {
                some(Value);
            }
            else
            {
                none();
            }
        }

        public void IfSome(Action<T> some)
        {
            if (HasValue)
            {
                some(Value);
            }
        }

        public Maybe<U> Map<U>(Func<T, Maybe<U>> map)
        {
            return HasValue
                ? map(Value)
                : Maybe<U>.None;
        }

        public Maybe<U> Map<U>(Func<T, U> map)
        {
            return HasValue
                ? Maybe.Some(map(Value))
                : Maybe<U>.None;
        }

        public static implicit operator Maybe<T>(T value)
        {
            return new Maybe<T>(new[] { value });
        }

        public static bool operator ==(Maybe<T> maybe, T value)
        {
            if (!maybe.HasValue)
                return false;

            return maybe.Value.Equals(value);
        }

        public static bool operator !=(Maybe<T> maybe, T value)
        {
            return !(maybe == value);
        }

        public static bool operator ==(Maybe<T> first, Maybe<T> second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(Maybe<T> first, Maybe<T> second)
        {
            return !(first == second);
        }
    }

    public static class Maybe
    {
        public static Maybe<T> Some<T>(T value)
        {
            return Maybe<T>.Some(value);
        }
    }
}
