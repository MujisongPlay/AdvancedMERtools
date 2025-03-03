using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using Exiled.API.Enums;
using System.Linq;
using Exiled.API.Features;
using InventorySystem.Items.Pickups;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;

namespace AdvancedMERTools
{

    [Serializable]
    public class Integer : Value
    {
        public int Value;
        public override void OnValidate() { }

        public override object GetValue(FunctionArgument args)
        {
            return Value;
        }
    }

    [Serializable]
    public class Real : Value
    {
        public float Value;
        public override void OnValidate() { }

        public override object GetValue(FunctionArgument args)
        {
            return Value;
        }
    }

    [Serializable]
    public class Bool : Value
    {
        public bool Value;
        public override void OnValidate() { }
        public override object GetValue(FunctionArgument args)
        {
            return Value;
        }
    }

    [Serializable]
    public class String : Value
    {
        public string Value;
        [Header("Replace {0}, {1}, ... to assgined values.")]
        public List<ScriptValue> StringInterpolations = new List<ScriptValue> { };
        public override void OnValidate()
        {
            StringInterpolations.ForEach(x => x.OnValidate());
        }

        public override object GetValue(FunctionArgument args)
        {
            string str = (string)Value.Clone();
            for (int i = 0; i < StringInterpolations.Count; i++)
            {
                str = str.Replace("{" + i.ToString() + "}", StringInterpolations[i].GetValue(args).ToString());
            }
            return str;
        }
    }

    [Serializable]
    public class Compare : Value
    {
        [Serializable]
        public enum CompareType
        {
            Equal,
            NotEqual,
            Bigger,
            BigOrEqual,
            Less,
            LessOrEqual,
            TypeEqual,
            And,
            Or,
            Xor,
            Not
        }

        public ScriptValue Value1;
        public CompareType Operator;
        public ScriptValue Value2;

        public override void OnValidate()
        {
            Value1.OnValidate();
            Value2.OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            object v1 = Value1.GetValue(args);
            object v2 = Value2.GetValue(args);
            if (v1 == null || v2 == null)
                return false;
            if (Operator == CompareType.TypeEqual)
                return v1.GetType() == v2.GetType();
            if ((v1 is int || v1 is float) && (v2 is int || v2 is float))
            {
                float V1 = Convert.ToSingle(v1);
                float V2 = Convert.ToSingle(v2);
                switch (Operator)
                {
                    case CompareType.Equal:
                        return V1 == V2;
                    case CompareType.NotEqual:
                        return V1 != V2;
                    case CompareType.Bigger:
                        return V1 > V2;
                    case CompareType.BigOrEqual:
                        return V1 >= V2;
                    case CompareType.Less:
                        return V1 < V2;
                    case CompareType.LessOrEqual:
                        return V1 <= V2;
                }
                return false;
            }
            if (v1 is bool && Operator == CompareType.Not)
                return !Convert.ToBoolean(v1);
            if (v1 is bool && v2 is bool)
            {
                bool V1 = Convert.ToBoolean(v1);
                bool V2 = Convert.ToBoolean(v2);
                switch (Operator)
                {
                    case CompareType.Equal:
                    case CompareType.BigOrEqual:
                    case CompareType.LessOrEqual:
                        return V1 == V2;
                    case CompareType.NotEqual:
                    case CompareType.Xor:
                        return V1 != V2;
                    case CompareType.And:
                        return V1 && V2;
                    case CompareType.Or:
                        return V1 || V2;
                }
                return false;
            }
            if (Operator == CompareType.Equal || Operator == CompareType.NotEqual)
            {
                if (v1.GetType() == v2.GetType())
                {
                    bool flag = v1.Equals(v2);
                    return Operator == CompareType.Equal ? flag : !flag;
                }
            }
            return false;
        }
    }

    [Serializable]
    public class IfThenElse : Value
    {
        public ScriptValue Statement;
        public ScriptValue Then;
        public ScriptValue Else;
        public override void OnValidate()
        {
            Statement.OnValidate();
            Then.OnValidate();
            Else.OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            return Statement.GetValue(args, false) ? Then.GetValue(args) : Else.GetValue(args);
        }
    }

    [Serializable]
    public class Array : Value
    {
        public ScriptValue[] Values;
        public override void OnValidate()
        {
            for (int i = 0; Values != null && i < Values.Length; i++)
                Values[i].OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            return Values.Select(x => x.GetValue(args)).ToArray();
        }
    }

    [Serializable]
    public class Variable : Value
    {
        public ScriptValue VariableName;
        public ScriptValue AccessLevel;
        public override void OnValidate()
        {
            VariableName.OnValidate();
            AccessLevel.OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            string str = VariableName.GetValue<string>(args, null);
            if (str == null)
                return null;
            switch (Math.Min(3, Math.Max(0, AccessLevel.GetValue(args, 0))))
            {
                case 0:
                    return args.FunctionVariables.TryGetValue(str, out object value) ? value : null;
                case 1:
                    return args.Function.ScriptVariables.TryGetValue(str, out value) ? value : null;
                case 2:
                    return AdvancedMERTools.Singleton.SchematicVariables[args.Function.OSchematic].TryGetValue(str, out value) ? value : null;
                case 3:
                    return AdvancedMERTools.Singleton.RoundVariable.TryGetValue(str, out value) ? value : null;
            }
            return null;
        }
    }

    [Serializable]
    public class Argument : Value
    {
        public ScriptValue ArgumentName;
        public override void OnValidate()
        {
            ArgumentName.OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            int index = args.Function.ArgumentsName.IndexOf(ArgumentName.GetValue(args, ""));
            return index == -1 || index >= args.Arguments.Count ? 0 : args.Arguments[index];
        }
    }

    [Serializable]
    public class VFunction : Value
    {
        public ScriptValue FunctionName;
        public List<ScriptValue> Arguments;
        public override void OnValidate()
        {
            FunctionName.OnValidate();
            Arguments.ForEach(x => x.OnValidate());
        }

        public override object GetValue(FunctionArgument args)
        {
            if (AdvancedMERTools.Singleton.FunctionExecutors[args.Function.OSchematic].TryGetValue(FunctionName.GetValue(args, ""), out FunctionExecutor function))
                return function.data.Execute(new FunctionArgument { Arguments = this.Arguments.Select(x => x.GetValue(args)).ToList(), player = args.player }).value;
            return null;
        }
    }

    [Serializable]
    public class Vector : Value
    {
        public ScriptValue X;
        public ScriptValue Y;
        public ScriptValue Z;
        public override void OnValidate()
        {
            X.OnValidate();
            Y.OnValidate();
            Z.OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            return new Vector3(X.GetValue(args, 0f), Y.GetValue(args, 0f), Z.GetValue(args, 0f));
        }
    }

    [Serializable]
    public class NumUnaryOp : Value
    {
        [Serializable]
        public enum NumUnaryOpType
        {
            Inverse,
            Reciprocal,
            Absolute,
            Factorial,
            Ceiling,
            Floor,
            Round,
            CommonLog,
            NaturalLog,
            BinaryLog,
            Exponential,
            Sine,
            Cosine,
            Tangent,
            Arcsine,
            Arccosine,
            Arctangent,
            Sigmoid,
            IsPrimeNumber
        }

        public NumUnaryOpType Operator;
        public ScriptValue Value;

        public override void OnValidate()
        {
            Value.OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            object v = Value.GetValue(args);
            if (v == null)
                return 0;
            if (v is int || v is float)
            {
                float V = Convert.ToSingle(v);
                switch (Operator)
                {
                    case NumUnaryOpType.Absolute:
                        return Mathf.Abs(V);
                    case NumUnaryOpType.Arccosine:
                        return Mathf.Acos(V);
                    case NumUnaryOpType.Arcsine:
                        return Mathf.Asin(V);
                    case NumUnaryOpType.Arctangent:
                        return Mathf.Atan(V);
                    case NumUnaryOpType.BinaryLog:
                        return Mathf.Log(V, 2);
                    case NumUnaryOpType.Ceiling:
                        return Mathf.Ceil(V);
                    case NumUnaryOpType.CommonLog:
                        return Mathf.Log10(V);
                    case NumUnaryOpType.Cosine:
                        return Mathf.Cos(V);
                    case NumUnaryOpType.Exponential:
                        return Mathf.Exp(V);
                    case NumUnaryOpType.Factorial:
                        return CalcHelper.Fact(V);
                    case NumUnaryOpType.Floor:
                        return Mathf.Floor(V);
                    case NumUnaryOpType.Inverse:
                        return -V;
                    case NumUnaryOpType.IsPrimeNumber:
                        if (Mathf.Round(V) != V)
                            return false;
                        for (int i = 2; i * i <= V; i++)
                            if (V % i == 0)
                                return false;
                        return true;
                    case NumUnaryOpType.NaturalLog:
                        return Mathf.Log(V, Mathf.Exp(1));
                    case NumUnaryOpType.Reciprocal:
                        if (V == 0)
                            return 0;
                        return 1f / V;
                    case NumUnaryOpType.Round:
                        return Mathf.Round(V);
                    case NumUnaryOpType.Sigmoid:
                        return 1f / (1f + Mathf.Exp(-V));
                    case NumUnaryOpType.Sine:
                        return Mathf.Sin(V);
                    case NumUnaryOpType.Tangent:
                        return Mathf.Tan(V);
                }
            }
            return 0;
        }
    }

    [Serializable]
    public class NumBinomialOp : Value
    {
        [Serializable]
        public enum NumBiOpType
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Modulo,
            RaiseToPower,
            Log,
            Max,
            Min,
            Permutation,
            Combination,
            Random,
            GCD,
            LCM,
        }

        public NumBiOpType Operator;
        public ScriptValue Value1;
        public ScriptValue Value2;

        public override void OnValidate()
        {
            Value1.OnValidate();
            Value2.OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            object v1 = Value1.GetValue(args);
            object v2 = Value2.GetValue(args);
            if (v1 == null || v2 == null)
                return 0;
            if ((v1 is int || v1 is float) && (v2 is int || v2 is float))
            {
                float i = Convert.ToSingle(v1);
                float j = Convert.ToSingle(v2);
                switch (Operator)
                {
                    case NumBiOpType.Add:
                        return i + j;
                    case NumBiOpType.Combination:
                        return CalcHelper.Fact(i) / Mathf.Max(CalcHelper.Fact(i - j) * CalcHelper.Fact(j), 1);
                    case NumBiOpType.Divide:
                        if (j == 0)
                            return 0;
                        if (v1 is int && v2 is int)
                            return (int)i / (int)j;
                        return i / j;
                    case NumBiOpType.GCD:
                        return CalcHelper.GCD(i, j);
                    case NumBiOpType.LCM:
                        return (int)Mathf.Abs(i * j) / CalcHelper.GCD(i, j);
                    case NumBiOpType.Log:
                        return Mathf.Log(j, i);
                    case NumBiOpType.Max:
                        return Mathf.Max(i, j);
                    case NumBiOpType.Min:
                        return Mathf.Min(i, j);
                    case NumBiOpType.Modulo:
                        if (j == 0)
                            return 0;
                        return i % j;
                    case NumBiOpType.Multiply:
                        return i * j;
                    case NumBiOpType.Permutation:
                        return CalcHelper.Fact(i) / Mathf.Max(1, CalcHelper.Fact(i - j));
                    case NumBiOpType.RaiseToPower:
                        return Mathf.Pow(i, j);
                    case NumBiOpType.Random:
                        if (v1 is int && v2 is int)
                            return UnityEngine.Random.Range((int)i, (int)j);
                        return UnityEngine.Random.Range(i, j);
                    case NumBiOpType.Subtract:
                        return i - j;
                }
            }
            return 0;
        }

    }

    public static class CalcHelper
    {
        public static int Fact(float V)
        {
            V = Mathf.Round(V);
            int pow = 1;
            for (int i = 2; i <= V; i++)
                pow *= i;
            return pow;
        }

        public static int GCD(float a, float b)
        {
            int A = Mathf.RoundToInt(a);
            int B = Mathf.RoundToInt(b);
            if (B == 0)
                return 0;
            if (A % B == 0)
                return B;
            return GCD(B, A % B);
        }
    }

    [Serializable]
    public class ArrUnaryOp : Value
    {
        [Serializable]
        public enum ArrUnaryOpType
        {
            Length,
            Reverse,
            GetRandomValue,
            Shuffled,
            RemovedNull,
            IsEmpty,
        }

        public ArrUnaryOpType Operator;
        public ScriptValue Array;

        public override void OnValidate()
        {
            Array.OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            object v = Array.GetValue(args);
            if (v != null && v is object[])
            {
                object[] val = v as object[];
                switch (Operator)
                {
                    case ArrUnaryOpType.GetRandomValue:
                        return val.RandomItem();
                    case ArrUnaryOpType.Length:
                        return val.Length;
                    case ArrUnaryOpType.RemovedNull:
                        return val.Where(x => x != null).ToArray();
                    case ArrUnaryOpType.Reverse:
                        return val.Select((x, y) => (x, y)).OrderBy(x => -x.y).Select(x => x.x).ToArray();
                    case ArrUnaryOpType.Shuffled:
                        return val.OrderBy(x => UnityEngine.Random.Range(int.MinValue, int.MaxValue)).ToArray();
                    case ArrUnaryOpType.IsEmpty:
                        return val.Length == 0;
                }
            }
            return null;
        }
    }

    [Serializable]
    public class ArrBinomialOp : Value
    {
        [Serializable]
        public enum ArrBiOpType
        {
            AppendToArray,
            RemoveFromArray,
            Skip,
            SkipLast,
            FilteredArray,
            MappedArray,
            SortedArray,
            TrueForAll,
            TrueForAny,
            Contains,
            ElementAt,
            IndexOfElement,
            Count,
            Max,
            Min,
            Sum,
            Product,
            Union,
            Intersect,
            DifferenceOfSets,
            Disjoint,
            Repeat,
        }

        public ArrBiOpType Operator;
        public ScriptValue Array;
        public ScriptValue EvaluationRule;

        public override void OnValidate()
        {
            Array.OnValidate();
            EvaluationRule.OnValidate();
        }

        ArrBiOpType[] Prevaluate = new ArrBiOpType[] 
        {
            ArrBiOpType.AppendToArray,
            ArrBiOpType.Contains,
            ArrBiOpType.DifferenceOfSets,
            ArrBiOpType.Disjoint,
            ArrBiOpType.ElementAt,
            ArrBiOpType.IndexOfElement,
            ArrBiOpType.Intersect,
            ArrBiOpType.RemoveFromArray,
            ArrBiOpType.Repeat,
            ArrBiOpType.Skip,
            ArrBiOpType.SkipLast,
            ArrBiOpType.Union
        };

        public override object GetValue(FunctionArgument args)
        {
            object v = Array.GetValue(args);
            if (v != null && v is object[])
            {
                object[] a = v as object[];
                if (Prevaluate.Contains(Operator))
                {
                    object s = EvaluationRule.GetValue(args);
                    if (s == null)
                        return a;
                    switch (Operator)
                    {
                        case ArrBiOpType.AppendToArray:
                            return a.Append(s).ToArray();
                        case ArrBiOpType.Contains:
                            return a.Contains(s);
                        case ArrBiOpType.DifferenceOfSets:
                            if (!(s is object[]))
                                return a;
                            return a.Except(s as object[]).ToArray();
                        case ArrBiOpType.Disjoint:
                            if (!(s is object[]))
                                return a;
                            return a.Union(s as object[]).Except(a.Intersect(s as object[])).ToArray();
                        case ArrBiOpType.ElementAt:
                            if (s is int || s is float)
                            {
                                int index = Mathf.RoundToInt(Convert.ToSingle(s));
                                index = Math.Min(a.Length - 1, Math.Max(0, index));
                                return a[index];
                            }
                            return a.First();
                        case ArrBiOpType.IndexOfElement:
                            return a.IndexOf(s);
                        case ArrBiOpType.Intersect:
                            if (!(s is object[]))
                                return a;
                            return a.Intersect(s as object[]).ToArray();
                        case ArrBiOpType.RemoveFromArray:
                            return a.Except(new object[] { s }).ToArray();
                        case ArrBiOpType.Repeat:
                            if (s is int || s is float)
                            {
                                int r = Mathf.RoundToInt(Convert.ToSingle(s));
                                List<object> list = new List<object> { };
                                for (int i = 0; i < r; i++)
                                    list.AddRange(a);
                                return list.ToArray();
                            }
                            return a;
                        case ArrBiOpType.Skip:
                            if (s is int || s is float)
                                return a.Skip(Mathf.RoundToInt(Convert.ToSingle(s))).ToArray();
                            return a;
                        case ArrBiOpType.SkipLast:
                            if (s is int || s is float)
                            {
                                a.Reverse();
                                a = a.Skip(Mathf.RoundToInt(Convert.ToSingle(s))).ToArray();
                                a.Reverse();
                                return a;
                            }
                            return a;
                        case ArrBiOpType.Union:
                            if (!(s is object[]))
                                return a;
                            return a.Union(s as object[]).ToArray();
                    }
                }
                else
                {
                    object[] evaluated = new object[a.Length];
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (i == 0)
                            args.Levels.Add((a[i], i));
                        else
                            args.Levels[args.Levels.Count - 1] = (a[i], i);
                        evaluated[i] = EvaluationRule.GetValue(args);
                    }
                    args.Levels.RemoveAt(args.Levels.Count - 1);
                    switch (Operator)
                    {
                        case ArrBiOpType.Count:
                            return a.Select((x, y) => (x, y)).Count(x => Convert.ToBoolean(evaluated[x.y]));
                        case ArrBiOpType.FilteredArray:
                            return a.Select((x, y) => (x, y)).Where(x => Convert.ToBoolean(evaluated[x.y])).Select(x => x.x).ToArray();
                        case ArrBiOpType.MappedArray:
                            return evaluated;
                        case ArrBiOpType.Max:
                            int index = 0;
                            float Max = float.MinValue;
                            for (int i = 0; i < a.Length; i++)
                            {
                                object ob = evaluated[i];
                                if (ob is int || ob is float)
                                {
                                    float ev = Convert.ToSingle(ob);
                                    if (ev > Max)
                                    {
                                        index = i;
                                        Max = ev;
                                    }
                                }
                            }
                            return a[index];
                        case ArrBiOpType.Min:
                            index = 0;
                            Max = float.MaxValue;
                            for (int i = 0; i < a.Length; i++)
                            {
                                object ob = evaluated[i];
                                if (ob is int || ob is float)
                                {
                                    float ev = Convert.ToSingle(ob);
                                    if (ev < Max)
                                    {
                                        index = i;
                                        Max = ev;
                                    }
                                }
                            }
                            return a[index];
                        case ArrBiOpType.Product:
                            float prod = 1;
                            for (int i = 0; i < a.Length; i++)
                            {
                                object ob = evaluated[i];
                                if (ob is int || ob is float)
                                    prod *= Convert.ToSingle(ob);
                            }
                            return prod;
                        case ArrBiOpType.SortedArray:
                            float[] eva = evaluated.Select(x => x is int || x is float ? Convert.ToSingle(x) : 0).ToArray();
                            return a.Select((x, y) => (x, y)).OrderBy(x => eva[x.y]).Select(x => x.x).ToArray();
                        case ArrBiOpType.Sum:
                            prod = 0;
                            for (int i = 0; i < a.Length; i++)
                            {
                                object ob = evaluated[i];
                                if (ob is int || ob is float)
                                    prod += Convert.ToSingle(ob);
                            }
                            return prod;
                        case ArrBiOpType.TrueForAll:
                            return a.All(x => x is bool ? Convert.ToBoolean(x) : false);
                        case ArrBiOpType.TrueForAny:
                            return a.Any(x => x is bool ? Convert.ToBoolean(x) : false);
                    }
                }
            }
            return 0;
        }
    }

    [Serializable]
    public class VecUnaryOp : Value
    {
        [Serializable]
        public enum VecUnaryOpType
        {
            X,
            Y,
            Z,
            Normalized,
            Magnitude,
            Inverse,
            DirectionFromAngles,
            AnglesFromDirection
        }

        public VecUnaryOpType Operator;
        public ScriptValue Vector;

        public override void OnValidate()
        {
            Vector.OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            object var = Vector.GetValue(args);
            Vector3 v = var == null || !(var is Vector3) ? Vector3.zero : (Vector3)var;
            switch (Operator)
            {
                case VecUnaryOpType.X:
                    return v.x;
                case VecUnaryOpType.Y:
                    return v.y;
                case VecUnaryOpType.Z:
                    return v.z;
                case VecUnaryOpType.Normalized:
                    return v == Vector3.zero ? Vector3.zero : v.normalized;
                case VecUnaryOpType.Magnitude:
                    return v.magnitude;
                case VecUnaryOpType.Inverse:
                    return new Vector3(-v.x, -v.y, -v.z);
                case VecUnaryOpType.DirectionFromAngles:
                    return Quaternion.Euler(v) * Vector3.forward;
                case VecUnaryOpType.AnglesFromDirection:
                    return v == Vector3.zero ? Vector3.zero : Quaternion.LookRotation(v, Vector3.up).eulerAngles;
            }
            return Vector3.zero;
        }
    }

    [Serializable]
    public class VecBinomialOp : Value
    {
        [Serializable]
        public enum VecBiOpType
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Distance,
            AngleBetweenVectors,
            AngleDifference,
            DotProduct,
            CrossProduct,
            DirectionTowards,
        }

        public VecBiOpType Operator;
        public ScriptValue Value1;
        public ScriptValue Value2;

        public override void OnValidate()
        {
            Value1.OnValidate();
            Value2.OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            object var1 = Value1.GetValue(args);
            object var2 = Value2.GetValue(args);
            if (var1 == null || var2 == null)
                return null;
            if (var1 is Vector3 && var2 is Vector3)
            {
                Vector3 v1 = (Vector3)var1;
                Vector3 v2 = (Vector3)var2;
                switch (Operator)
                {
                    case VecBiOpType.Add:
                        return v1 + v2;
                    case VecBiOpType.AngleBetweenVectors:
                        return Vector3.Angle(v1, v2);
                    case VecBiOpType.AngleDifference:
                        return new Vector3(Mathf.DeltaAngle(v1.x, v2.x), Mathf.DeltaAngle(v1.y, v2.y), Mathf.DeltaAngle(v1.z, v2.z)).magnitude;
                    case VecBiOpType.CrossProduct:
                        return Vector3.Cross(v1, v2);
                    case VecBiOpType.DirectionTowards:
                        return (v2 - v1).normalized;
                    case VecBiOpType.Distance:
                        return Vector3.Distance(v1, v2);
                    case VecBiOpType.Divide:
                        return new Vector3(v1.x / (v2.x == 0 ? 1 : v2.x), v1.y / (v2.y == 0 ? 1 : v2.y), v1.z / (v2.z == 0 ? 1 : v2.z));
                    case VecBiOpType.DotProduct:
                        return Vector3.Dot(v1, v2);
                    case VecBiOpType.Multiply:
                        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
                    case VecBiOpType.Subtract:
                        return v1 - v2;
                }
            }
            else if ((var1 is int || var2 is float) && var2 is Vector3)
            {
                Vector3 v1 = (Vector3)var2;
                float val = Convert.ToSingle(var1);
                switch (Operator)
                {
                    case VecBiOpType.Multiply:
                        return v1 * val;
                }
            }
            else if ((var2 is int || var2 is float) && var1 is Vector3)
            {
                Vector3 v1 = (Vector3)var1;
                float val = Convert.ToSingle(var2);
                switch (Operator)
                {
                    case VecBiOpType.Multiply:
                        return v1 * val;
                    case VecBiOpType.Divide:
                        return v1 / val;
                }
            }
            return null;
        }
    }

    [Serializable]
    public class StrUnaryOp : Value
    {
        [Serializable]
        public enum StrUnaryOpType
        {
            Length,
            Upper,
            Lower,
            UpperLowerSwitch,
            Strip,
            Trim,
            ToInteger,
            ToReal,
            ToCharArray,
            ToPlayerAsName,
            ToItemAsName
        }

        public StrUnaryOpType Operator;
        public ScriptValue String;

        public override void OnValidate()
        {
            String.OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            string str = String.GetValue(args, "");
            switch (Operator)
            {
                case StrUnaryOpType.Length:
                    return str.Length;
                case StrUnaryOpType.Lower:
                    return str.ToLower();
                case StrUnaryOpType.Strip:
                    return str.Replace(" ", "");
                case StrUnaryOpType.ToCharArray:
                    return str.ToCharArray();
                case StrUnaryOpType.ToInteger:
                    return int.TryParse(str, out int Vi) ? Vi : 0;
                case StrUnaryOpType.ToReal:
                    return float.TryParse(str, out float Vf) ? Vf : 0f;
                case StrUnaryOpType.Trim:
                    return str.Trim();
                case StrUnaryOpType.Upper:
                    return str.ToUpper();
                case StrUnaryOpType.UpperLowerSwitch:
                    char[] vs = str.ToCharArray();
                    for (int i = 0; i < vs.Length; i++)
                    {
                        if ('A' <= vs[i] && vs[i] <= 'Z')
                            vs[i] = (char)(vs[i] - 'A' + 'a');
                        else if ('a' <= vs[i] && vs[i] <= 'z')
                            vs[i] = (char)(vs[i] - 'a' + 'A');
                    }
                    return new string(vs);
                case StrUnaryOpType.ToPlayerAsName:
                    if (Player.List.Any(x => x.Nickname == str))
                    {
                        return Player.List.First(x => x.Nickname == str);
                    }
                    return null;
                case StrUnaryOpType.ToItemAsName:
                    if (Enum.TryParse(str, out ItemType type))
                        return type;
                    return null;
                    
            }
            return str;
        }
    }

    [Serializable]
    public class StrBinomialOp : Value
    {
        [Serializable]
        public enum StrBinomialOpType
        {
            Add,
            Repeat,
            Contains,
            Split,
            Skip,
            SkipLast,
        }

        public StrBinomialOpType Operator;
        public ScriptValue String;
        public ScriptValue EvaluationRule;

        public override void OnValidate()
        {
            String.OnValidate();
            EvaluationRule.OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            object v1 = String.GetValue(args);
            object v2 = EvaluationRule.GetValue(args);
            if (v1 == null || !(v1 is string))
                return null;
            string str = Convert.ToString(v1);
            if (v2 is int || v2 is float)
            {
                float f = Convert.ToSingle(v2);
                switch (Operator)
                {
                    case StrBinomialOpType.Repeat:
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        for (int i = 0; i < f; i++)
                            sb.Append(str);
                        return sb.ToString();
                    case StrBinomialOpType.Skip:
                        return new string(str.Skip(Mathf.RoundToInt(f)).ToArray());
                    case StrBinomialOpType.SkipLast:
                        return new string(str.Reverse().Skip(Mathf.RoundToInt(f)).Reverse().ToArray());
                }
            }
            else if (v2 is string)
            {
                string st = Convert.ToString(v2);
                switch (Operator)
                {
                    case StrBinomialOpType.Add:
                        return str + st;
                    case StrBinomialOpType.Contains:
                        return str.Contains(st);
                    case StrBinomialOpType.Split:
                        return str.Split(new string[] { st }, int.MaxValue, StringSplitOptions.None);
                }
            }
            return str;
        }
    }

    [Serializable]
    public class ArrayEvaluateHelper : Value
    {
        [Serializable]
        public enum AEVHelperType
        {
            CurrentEvaluateElement,
            CurrentEvaluateIndex
        }

        public AEVHelperType Value;
        [Header("Increase with its nested level from 0.")]
        public ScriptValue AccessLevel;

        public override void OnValidate()
        {
            AccessLevel.OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            object v = AccessLevel.GetValue(args);
            int a = v == null || !(v is int || v is float) ? 0 : Mathf.RoundToInt(Convert.ToSingle(v));
            a = Math.Min(args.Levels.Count - 1, Math.Max(0, a));
            return Value == AEVHelperType.CurrentEvaluateElement ? args.Levels[a].Item1 : args.Levels[a].Item2;
        }
    }

    [Serializable]
    public class ConstValue : Value
    {
        [Serializable]
        public enum ConstValueType
        {
            PI,
            E,
            GoldenRatio,
            C,
            TheAnswerToLifeTheUniverseAndEverything
        }

        public ConstValueType Value;

        public override void OnValidate()
        {
        }

        public override object GetValue(FunctionArgument args)
        {
            switch (Value)
            {
                case ConstValueType.C:
                    return 299792458;
                case ConstValueType.E:
                    return Mathf.Exp(1);
                case ConstValueType.GoldenRatio:
                    return (1f + (float)Math.Pow(5, 0.5)) / 2f;
                case ConstValueType.PI:
                    return Mathf.PI;
                case ConstValueType.TheAnswerToLifeTheUniverseAndEverything:
                default:
                    return 42;
            }
        }
    }

    [Serializable]
    public class EvaluateOnce : Value
    {
        public ScriptValue Value;

        public override void OnValidate()
        {
            Value.OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            if (flag)
                return valued;
            flag = true;
            return valued = Value.GetValue(args);
        }

        bool flag = false;
        object valued = null;
    }

    [Serializable]
    public class VCollisionType : Value
    {
        public CollisionType Value;

        public override void OnValidate()
        {
        }

        public override object GetValue(FunctionArgument args)
        {
            return Value;
        }
    }

    [Serializable]
    public class CollisionDetectTarget : Value
    {
        public DetectType Value;

        public override void OnValidate()
        {
        }

        public override object GetValue(FunctionArgument args)
        {
            return Value;
        }
    }

    [Serializable]
    public class EffectActionType : Value
    {
        public EffectFlagE Value;

        public override void OnValidate()
        {
        }

        public override object GetValue(FunctionArgument args)
        {
            return Value;
        }
    }

    [Serializable]
    public class VEffectType : Value
    {
        public EffectType Value;

        public override void OnValidate()
        {
        }

        public override object GetValue(FunctionArgument args)
        {
            return Value;
        }
    }

    //[Serializable]
    //public class VTeleportInvokeType : Value
    //{
    //    public TeleportInvokeType Value;

    //    public override void OnValidate()
    //    {
    //    }

    //    public override object GetValue(FunctionArgument args)
    //    {
    //        return Value;
    //    }
    //}

    [Serializable]
    public class VWarheadActionType : Value
    {
        public WarheadActionType Value;

        public override void OnValidate()
        {
        }

        public override object GetValue(FunctionArgument args)
        {
            return Value;
        }
    }

    [Serializable]
    public class AnimationActionType : Value
    {
        public AnimationTypeE Value;

        public override void OnValidate()
        {
        }

        public override object GetValue(FunctionArgument args)
        {
            return Value;
        }
    }

    [Serializable]
    public class VParameterType : Value
    {
        public ParameterTypeE Value;

        public override void OnValidate()
        {
        }

        public override object GetValue(FunctionArgument args)
        {
            return Value;
        }
    }

    [Serializable]
    public class VMessageType : Value
    {
        public MessageTypeE Value;

        public override void OnValidate()
        {
        }

        public override object GetValue(FunctionArgument args)
        {
            return Value;
        }
    }

    [Serializable]
    public class PlayerArray : Value
    {
        public enum PlayerArrayType
        {
            AllPlayers,
            AlivePlayers,
            Scps,
            ScpsExcludeScp0492,
            Mtfs,
            Chaos,
            Dclass,
            Scientist,
            Spectators,
            Guards,
            FoundationSide,
            AntiFoundationSide,
            Humans,
        }

        public PlayerArrayType ArrayType;

        public override void OnValidate()
        {
        }

        public override object GetValue(FunctionArgument args)
        {
            switch (ArrayType)
            {
                case PlayerArrayType.AlivePlayers:
                    return Player.List.Where(x => x.IsAlive).ToArray();
                case PlayerArrayType.AllPlayers:
                    return Player.List.ToArray();
                case PlayerArrayType.AntiFoundationSide:
                    return Player.List.Where(x => x.LeadingTeam != LeadingTeam.FacilityForces).ToArray();
                case PlayerArrayType.Chaos:
                    return Player.List.Where(x => x.LeadingTeam == LeadingTeam.ChaosInsurgency && x.Role.Type != PlayerRoles.RoleTypeId.ClassD).ToArray();
                case PlayerArrayType.Dclass:
                    return Player.List.Where(x => x.Role.Type == PlayerRoles.RoleTypeId.ClassD).ToArray();
                case PlayerArrayType.FoundationSide:
                    return Player.List.Where(x => x.LeadingTeam == LeadingTeam.FacilityForces).ToArray();
                case PlayerArrayType.Guards:
                    return Player.List.Where(x => x.Role.Type == PlayerRoles.RoleTypeId.FacilityGuard).ToArray();
                case PlayerArrayType.Humans:
                    return Player.List.Where(x => x.IsHuman).ToArray();
                case PlayerArrayType.Mtfs:
                    return Player.List.Where(x => x.Role.Team == PlayerRoles.Team.FoundationForces && x.Role.Type != PlayerRoles.RoleTypeId.FacilityGuard).ToArray();
                case PlayerArrayType.Scientist:
                    return Player.List.Where(x => x.Role.Type == PlayerRoles.RoleTypeId.Scientist);
                case PlayerArrayType.Scps:
                    return Player.List.Where(x => x.Role.Team == PlayerRoles.Team.SCPs);
                case PlayerArrayType.ScpsExcludeScp0492:
                    return Player.List.Where(x => x.Role.Team == PlayerRoles.Team.SCPs && x.Role.Type != PlayerRoles.RoleTypeId.Scp0492);
                case PlayerArrayType.Spectators:
                    return Player.List.Where(x => x.Role.Type == PlayerRoles.RoleTypeId.Spectator).ToArray();
            }
            return null;
        }
    }

    [Serializable]
    public class SingleTarget : Value
    {
        public enum SingleTargetType
        {
            EventPlayer,
            SchematicEntity,
            ScriptEntity,
        };

        public SingleTargetType TargetType;

        public override void OnValidate()
        {
        }

        public override object GetValue(FunctionArgument args)
        {
            switch (TargetType)
            {
                case SingleTargetType.EventPlayer:
                    return args.player;
                case SingleTargetType.SchematicEntity:
                    return args.schematic.gameObject;
                case SingleTargetType.ScriptEntity:
                    return args.transform.gameObject;
            }
            return null;
        }
    }

    [Serializable]
    public class VScp914Mode : Value
    {
        public Scp914Mode Value;

        public override void OnValidate()
        {
        }

        public override object GetValue(FunctionArgument args)
        {
            return Value;
        }
    }

    [Serializable]
    public class VItemType : Value
    {
        public ItemType Value;

        public override void OnValidate()
        {
        }

        public override object GetValue(FunctionArgument args)
        {
            return Value;
        }
    }

    [Serializable]
    public class VRoleType : Value
    {
        public PlayerRoles.RoleTypeId Value;

        public override void OnValidate()
        {
        }

        public override object GetValue(FunctionArgument args)
        {
            return Value;
        }
    }

    [Serializable]
    public class ItemUnaryOp : Value
    {
        [Serializable]
        public enum ItemUnaryOpType
        {
            ItemType,
            Entity,
            Owner,
            PrevOwner
        }

        public ScriptValue ItemOrPickup;
        public ItemUnaryOpType Operator;

        public override void OnValidate()
        {
            ItemOrPickup.OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            Item item = ItemOrPickup.GetValue<Item>(args, null);
            if (item != null)
            {
                switch (Operator)
                {
                    case ItemUnaryOpType.ItemType:
                        return item.Type;
                    case ItemUnaryOpType.Owner:
                        return item.Owner;
                }
                return null;
            }
            ItemPickupBase pickup = ItemOrPickup.GetValue<ItemPickupBase>(args, null);
            if (pickup != null)
            {
                switch (Operator)
                {
                    case ItemUnaryOpType.ItemType:
                        return pickup.Info.ItemId;
                    case ItemUnaryOpType.PrevOwner:
                        return pickup.PreviousOwner;
                }
                return null;
            }
            return null;
        }
    }

    [Serializable]
    public class PlayerUnaryOp : Value
    {
        [Serializable]
        public enum PlayerUnaryOpType
        {
            AHP,
            Cuffer,
            CurrentItem,
            CurrentSpectatingPlayers,
            CustomInfo,
            CustomName,
            DisplayNickname,
            GroupName,
            HP,
            HumeShield,
            Id,
            IsAlive,
            IsCHI,
            IsCuffed,
            IsDead,
            IsFoundationSide,
            IsFFon,
            IsHuman,
            IsInPocketDimension,
            IsInventoryEmpty,
            IsInventoryFull,
            IsJumping,
            IsNPC,
            IsNTF,
            IsReloading,
            IsScp,
            IsSpeaking,
            IsTutorial,
            IsUsingStamina,
            Items,
            MaxAHP,
            MaxHP,
            MaxHumeShield,
            Position,
            Role,
            FacingDirection,
            Scale,
            Stamina,
            EntityObject,
            UniqueRole,
            Velocity,
        }

        public PlayerUnaryOpType Operator;
        public ScriptValue Player;

        public override void OnValidate()
        {
            Player.OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            Player p = Player.GetValue<Player>(args, null);
            if (p == null)
                return null;
            switch (Operator)
            {
                case PlayerUnaryOpType.AHP:
                    return p.ArtificialHealth;
                case PlayerUnaryOpType.Cuffer:
                    return p.Cuffer;
                case PlayerUnaryOpType.CurrentItem:
                    return p.CurrentItem;
                case PlayerUnaryOpType.CurrentSpectatingPlayers:
                    return p.CurrentSpectatingPlayers.ToArray();
                case PlayerUnaryOpType.CustomInfo:
                    return p.CustomInfo;
                case PlayerUnaryOpType.CustomName:
                    return p.CustomName;
                case PlayerUnaryOpType.DisplayNickname:
                    return p.DisplayNickname;
                case PlayerUnaryOpType.EntityObject:
                    return p.GameObject;
                case PlayerUnaryOpType.FacingDirection:
                    return p.CameraTransform.forward;
                case PlayerUnaryOpType.GroupName:
                    return p.GroupName;
                case PlayerUnaryOpType.HP:
                    return p.Health;
                case PlayerUnaryOpType.HumeShield:
                    return p.HumeShield;
                case PlayerUnaryOpType.Id:
                    return p.Id;
                case PlayerUnaryOpType.IsAlive:
                    return p.IsAlive;
                case PlayerUnaryOpType.IsCHI:
                    return p.IsCHI;
                case PlayerUnaryOpType.IsCuffed:
                    return p.IsCuffed;
                case PlayerUnaryOpType.IsDead:
                    return p.IsDead;
                case PlayerUnaryOpType.IsFFon:
                    return p.IsFriendlyFireEnabled;
                case PlayerUnaryOpType.IsFoundationSide:
                    return p.Role.Side == Side.Mtf;
                case PlayerUnaryOpType.IsHuman:
                    return p.IsHuman;
                case PlayerUnaryOpType.IsInPocketDimension:
                    return p.IsInPocketDimension;
                case PlayerUnaryOpType.IsInventoryEmpty:
                    return p.IsInventoryEmpty;
                case PlayerUnaryOpType.IsInventoryFull:
                    return p.IsInventoryFull;
                case PlayerUnaryOpType.IsJumping:
                    return p.IsJumping;
                case PlayerUnaryOpType.IsNPC:
                    return p.IsNPC;
                case PlayerUnaryOpType.IsNTF:
                    return p.IsNTF;
                case PlayerUnaryOpType.IsReloading:
                    return p.CurrentItem != null && p.CurrentItem.Category == ItemCategory.Firearm && (p.CurrentItem as Exiled.API.Features.Items.Firearm).IsReloading;
                case PlayerUnaryOpType.IsScp:
                    return p.IsScp;
                case PlayerUnaryOpType.IsSpeaking:
                    return p.IsSpeaking;
                case PlayerUnaryOpType.IsTutorial:
                    return p.IsTutorial;
                case PlayerUnaryOpType.IsUsingStamina:
                    return p.IsUsingStamina;
                case PlayerUnaryOpType.Items:
                    return p.Items.ToArray();
                case PlayerUnaryOpType.MaxAHP:
                    return p.MaxArtificialHealth;
                case PlayerUnaryOpType.MaxHP:
                    return p.MaxHealth;
                case PlayerUnaryOpType.MaxHumeShield:
                    return p.MaxHumeShield;
                case PlayerUnaryOpType.Position:
                    return p.Position;
                case PlayerUnaryOpType.Role:
                    return p.Role.Type;
                case PlayerUnaryOpType.Scale:
                    return p.Scale;
                case PlayerUnaryOpType.Stamina:
                    return p.Stamina;
                case PlayerUnaryOpType.UniqueRole:
                    return p.UniqueRole;
                case PlayerUnaryOpType.Velocity:
                    return p.Velocity;
            }
            return p;
        }
    }

    [Serializable]
    public class EntityUnaryOp : Value
    {
        [Serializable]
        public enum EntityUnaryOpType
        {
            Name,
            Position,
            Rotation,
            Scale,
            Parent,
            IsActive,
            ChildCount,
            ToPlayer,
            ToItemPickup,
        }

        public EntityUnaryOpType Operator;
        public ScriptValue Entity;

        public override void OnValidate()
        {
            Entity.OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            GameObject game = Entity.GetValue<GameObject>(args, null);
            switch (Operator)
            {
                case EntityUnaryOpType.ChildCount:
                    return game.transform.childCount;
                case EntityUnaryOpType.IsActive:
                    return game.activeInHierarchy;
                case EntityUnaryOpType.Name:
                    return game.name;
                case EntityUnaryOpType.Parent:
                    return game.transform.parent;
                case EntityUnaryOpType.Position:
                    return game.transform.position;
                case EntityUnaryOpType.Rotation:
                    return game.transform.rotation.eulerAngles;
                case EntityUnaryOpType.Scale:
                    return game.transform.localScale;
                case EntityUnaryOpType.ToItemPickup:
                    return Pickup.Get(game);
                case EntityUnaryOpType.ToPlayer:
                    return Player.Get(game);
            }
            return game;
        }
    }

    [Serializable]
    public class EntityBinomialOp : Value
    {
        [Serializable]
        public enum EntityBinomialOpType
        {
            GetChildAt,
            WorldToLocal,
            LocalToWorld
        }

        public EntityBinomialOpType Operator;
        public ScriptValue Entity;
        public ScriptValue Value;

        public override void OnValidate()
        {
            Entity.OnValidate();
            Value.OnValidate();
        }

        public override object GetValue(FunctionArgument args)
        {
            GameObject game = Entity.GetValue<GameObject>(args, null);
            if (game == null)
                return null;
            switch (Operator)
            {
                case EntityBinomialOpType.GetChildAt:
                    int T = Value.GetValue(args, 0);
                    return game.transform.GetChild(T);
                case EntityBinomialOpType.LocalToWorld:
                    Vector3 vec = Value.GetValue(args, Vector3.zero);
                    return game.transform.TransformPoint(vec);
                case EntityBinomialOpType.WorldToLocal:
                    vec = Value.GetValue(args, Vector3.zero);
                    return game.transform.InverseTransformPoint(vec);
            }
            return null;
        }
    }
}
