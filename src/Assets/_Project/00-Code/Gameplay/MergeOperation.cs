using UnityEngine;

namespace Mergeburgers.Gameplay
{
    public enum MergeOperationType
    {
        None = 0,
        Move = 1, // плитка переместилась с одной позиции на другую
        Merge = 2, // две плитки слились в одну (исчезли source, появилась result в position)
    }
    
    /// <summary>
    /// Одна атомарная операция, произошедшая после swipe.
    /// MergeResolver возвращает упорядоченный список таких операций,
    /// чтобы UI/анимации могли проигрывать их последовательно (Day 14).
    /// </summary>

    public readonly struct MergeOperation
    {
        public MergeOperationType Type { get; }
        public Vector2Int From { get; }       // откуда взялось
        public Vector2Int To { get; }         // куда стало
        public IngredientType ResultType { get; } // что получилось (для Move совпадает с типом исходной плитки)

        public MergeOperation(MergeOperationType type, Vector2Int from, Vector2Int to, IngredientType resultType)
        {
            Type = type;
            From = from;
            To = to;
            ResultType = resultType;
        }
    }
}