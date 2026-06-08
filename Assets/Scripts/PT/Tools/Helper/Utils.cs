using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Libraries.RSG;
using PT.Tools.Debugging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using IPromise = Libraries.RSG.IPromise;
using Random = UnityEngine.Random;

namespace PT.Tools.Helper
{
    public enum DirectionEnum { Left, Right, Down, Up }

    public static class Utils
    {
        #region Important

        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }

        public static T[] GetRange<T>(this T[] array, int index, int count)
        {
            T[] tmpArray = new T[count];
            Array.Copy(array, index, tmpArray, 0, count);
            return tmpArray;
        }

        public static void SetActive(this Component m, bool active)
        {
            if (m && m.gameObject) m.gameObject.SetActive(active);
        }
    
        public static void SetActive(this GameObject[] objects, bool active)
        {
            foreach (var obj in objects) obj.SetActive(active);
        }

        #endregion
        #region Direction 

        public static Vector2 GetNormalizedDiagonalDirection(Vector2 direction)
        {
            direction.Normalize();

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;

            float snappedAngle = Mathf.Round(angle / 45) * 45;
            if (snappedAngle == 360) snappedAngle = 0;

            return GetAngleToDirection(snappedAngle);
        }
        private static Vector2 GetAngleToDirection(float angle)
        {
            switch (angle)
            {
                case 0: return Vector2.right;
                case 45: return new Vector2(0.75f, 0.75f);
                case 90: return Vector2.up;
                case 135: return new Vector2(-0.75f, 0.75f);
                case 180: return Vector2.left;
                case 225: return new Vector2(-0.75f, -0.75f);
                case 270: return Vector2.down;
                case 315: return new Vector2(0.75f, -0.75f);
                default: DebugManager.Log(DebugCategory.Misc, "normalized angle of 8 points direction is 0"); return Vector2.up;
            }
        }

        public static void NormalizeIntoCardinalDirection(this ref Vector2 direction)
        {
            direction.Normalize();

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                direction = new Vector2(direction.x > 0 ? 1 : -1, 0);
            }
            else //if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
            {
                direction = new Vector2(0, direction.y > 0 ? 1 : -1);
            }
        }
        
        public static bool IsDirectionCardinal(Vector2 direction) =>
            Mathf.Abs(direction.x) == 1 || Mathf.Abs(direction.y) == 1;

        public static Vector3 NormalizeDirectionTo(this Vector3 positionFrom, Vector3 positionTo) =>
            (positionTo - positionFrom).normalized;
        public static Vector3 NormalizeDirectionTo(this Vector2 positionFrom, Vector2 positionTo) =>
            (positionTo - positionFrom).normalized;

        public static DirectionEnum GetDirectionEnumFromCardinalDirection(Vector2 direction)
        {
            switch (direction)
            {
                case Vector2 { x: -1, y: 0 }: return DirectionEnum.Left;
                case Vector2 { x: 1, y: 0 }: return DirectionEnum.Right;
                case Vector2 { x: 0, y: -1 }: return DirectionEnum.Down;
                case Vector2 { x: 0, y: 1 }: return DirectionEnum.Up;

                default: DebugManager.Log(DebugCategory.Misc, "direction was not cardinal"); return DirectionEnum.Up;
            }
        }
        public static Vector2 GetDirection(DirectionEnum directionEnum)
        {
            switch (directionEnum)
            {
                case DirectionEnum.Left: return new(-1, 0);
                case DirectionEnum.Right: return new(1, 0);
                case DirectionEnum.Down: return new(0, -1);
                case DirectionEnum.Up: return new(0, 1);
                default: return Vector2.zero;
            }
        }
        public static Vector2Int GetDirectionInt(DirectionEnum directionEnum)
        {
            switch (directionEnum)
            {
                case DirectionEnum.Left: return new(-1, 0);
                case DirectionEnum.Right: return new(1, 0);
                case DirectionEnum.Down: return new(0, -1);
                case DirectionEnum.Up: return new(0, 1);
                default: return Vector2Int.zero;
            }
        }

        #endregion
        #region Misc

        public static void AdjustValueBetweenMinMax(int min, int max, ref int value) => value = Math.Max(min, Math.Min(value, max));
        public static int AdjustValueBetweenMinMax(int min, int max, int value) => value = Math.Max(min, Math.Min(value, max));
        public static float AdjustValueBetweenMinMax(float min, float max, float value) => value = Mathf.Max(min, Mathf.Min(value, max));
        public static bool ValueIsInBetweenValues(int start, int end, int value) => value >= start && value < end;

        public static bool ContainsPositionWithOffset(List<Vector2> positions, Vector2 targetPosition, float offset)
            => positions.Any(x => Vector2.Distance(x, targetPosition) < offset);
        public static Vector2 GetPositionCloseToOffset(List<Vector2> positions, Vector2 targetPosition, float offset)
            => positions.FirstOrDefault(x => Vector2.Distance(x, targetPosition) < offset);

        public static Vector2 GetMultipliedVector(Vector2 value, Vector2 multiplier)
            => new(value.x * multiplier.x, value.y * multiplier.y);

        public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
        {
            Vector3 deltaPosition = rectTransform.pivot - pivot;
            deltaPosition.Scale(rectTransform.rect.size);
            deltaPosition.Scale(rectTransform.localScale);
            deltaPosition = rectTransform.rotation * deltaPosition;

            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }

        public static List<T> Clone<T>(this List<T> oldList)
        {
            return new List<T>(oldList);
        }

        public static Dictionary<T1, T2> Clone<T1, T2>(this Dictionary<T1, T2> oldDictionary)
        {
            return oldDictionary.ToDictionary(entry => entry.Key,
                entry => entry.Value);
        }

        public static T Shift<T>(this List<T> list)
        {
            if (list.Count > 0)
            {
                var result = list[0];
                list.RemoveAt(0);
                return result;
            }
            return default(T);
        }

        public static void ResolveOnce(this Promise p)
        {
            if (p.CurState == PromiseState.Pending)
                p.Resolve();
        }
        public static void RejectOnce(this Promise p, Exception e = null)
        {
            if (p.CurState == PromiseState.Pending)
                p.Reject(e);
        }

        #endregion
        #region Random 
        public static float GetRandomValue(this Vector2 minMax) =>
            minMax.x > minMax.y? Random.Range(minMax.y, minMax.x) : Random.Range(minMax.x, minMax.y);
        public static int GetRandomValue(this Vector2Int minMax) =>
            minMax.x > minMax.y? Random.Range(minMax.y, minMax.x) : Random.Range(minMax.x, minMax.y);
        public static float GetRandomValue(float min, float max) => Random.Range(min, max);
        public static int GetRandomValueInt(int min, int max) => Random.Range(min, max);
        public static int GetRandomValueTo(int max) => Random.Range(0, max);
        public static float GetRandomNext(float max) => Random.Range(0, max);
        public static int GetRandomNextInt(int max) => Random.Range(0, max);
        public static T GetRandomEnum<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(GetRandomNextInt(values.Length));
        }
        public static T GetRandomElement<T>(this IList<T> elements) => elements[GetRandomNextInt(elements.Count)];
        public static T GetRandomElement<T>(this T[] elements) => elements[GetRandomNextInt(elements.Length)];
        public static T GetRandomElement<T>(this List<T> elements) => elements[GetRandomNextInt(elements.Count)];
        public static Vector3 GetRandomDirection()
        {
            var result = new Vector3(GetRandomValue(new Vector2(-1, 1)), GetRandomValue(new Vector2(-1, 1)), GetRandomValue(new Vector2(-1, 1)));

            return result.normalized;
        }
        public static Vector3 GetRandomBetweenVectors(Vector3 min, Vector3 max)
        {
            return new(GetRandomValue(min.x, max.x), GetRandomValue(min.y, max.y), GetRandomValue(min.z, max.z));
        }
        public static Vector2 GetRandomVector(float value) => new(GetRandomValue(-value, value), GetRandomValue(-value, value));
        
        public static T PickByWeight<T>(List<(T item, float weight)> weightedList)
        {
            float total = 0f;
            foreach (var w in weightedList) total += w.weight;

            float rand = GetRandomNext(total);
            float cumulative = 0f;
            foreach (var (item, weight) in weightedList)
            {
                cumulative += weight;
                if (rand <= cumulative) return item;
            }

            return default;
        }

        #endregion
        #region UI
        
        public static void ResizeSpriteToScreen(SpriteRenderer sprite, UnityEngine.Camera theCamera, int fitToScreenWidth = 1, int fitToScreenHeight = 1)
        {
            sprite.transform.localScale = new Vector3(1, 1, 1);

            var width = sprite.sprite.bounds.size.x;
            var height = sprite.sprite.bounds.size.y;

            var worldScreenHeight = theCamera.orthographicSize * 2.0f;
            var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            if (fitToScreenWidth != 0)
            {
                var sizeX = new Vector2(worldScreenWidth / width / fitToScreenWidth, sprite.transform.localScale.y);
                sprite.transform.localScale = sizeX;
            }

            if (fitToScreenHeight != 0)
            {
                var sizeY = new Vector2(sprite.transform.localScale.x, worldScreenHeight / height / fitToScreenHeight);
                sprite.transform.localScale = sizeY;
            }
        }

        #endregion
        #region Animation 

        public static IPromise ToPromise(this TweenerCore<Vector3, Vector3, VectorOptions> tween)
        {
            var promise = new Promise();
            var onWasComplete = tween.onComplete;
            var onWasKill = tween.onKill;
            tween.OnComplete(() =>
            {
                onWasComplete?.Invoke();
                promise.ResolveOnce();
            });
            tween.OnKill(() =>
            {
                onWasKill?.Invoke();
                promise.RejectOnce();
            });
            return promise;
        }

        public static IPromise ToPromise(this TweenerCore<float, float, FloatOptions> tween)
        {
            var promise = new Promise();
            tween.OnComplete(promise.ResolveOnce);
            tween.OnKill(() => promise.RejectOnce());
            return promise;
        }

        public static IPromise ToPromise(this Tween tween)
        {
            var promise = new Promise();
            var onWasComplete = tween.onComplete;
            var onWasKill = tween.onKill;
            tween.OnComplete(() =>
            {
                onWasComplete?.Invoke();
                promise.ResolveOnce();
            });
            tween.OnKill(() =>
            {
                onWasKill?.Invoke();
                promise.RejectOnce();
            });
            return promise;
        }

        public static IPromise ToPromise(this Sequence sequence)
        {
            var promise = new Promise();
            var onWasComplete = sequence.onComplete;
            var onWasKill = sequence.onKill;
            sequence.OnComplete(() =>
            {
                onWasComplete?.Invoke();
                promise.ResolveOnce();
            });
            sequence.OnKill(() =>
            {
                onWasKill?.Invoke();
                promise.RejectOnce();
            });
            return promise;
        }

        public static IPromise ToPromise(this TweenerCore<Quaternion, Vector3, QuaternionOptions> tween)
        {
            var promise = new Promise();
            tween.OnComplete(promise.Resolve);
            return promise;
        }
        
        public static UniTask ToUniTask(this IPromise promise)
        {
            var completionSource = new UniTaskCompletionSource();
        
            promise.Then(() => completionSource.TrySetResult());
        
            return completionSource.Task;
        }

        #endregion
        #region Formatting
        public static bool CloneFile(string from, string to)
        {
            var fromDist = Application.persistentDataPath + "/" + from;
            var toDist = Application.persistentDataPath + "/" + to;
            if (!File.Exists(fromDist))
                return false;

            File.Copy(Application.persistentDataPath + "/" + from,
                Application.persistentDataPath + "/" + to);
            return true;
        }
        
        public static string DictToString<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            if (dict == null || dict.Count == 0) return "{}";
            return "{ " + string.Join(", ", dict.Select(kvp => $"{kvp.Key}:{kvp.Value}")) + " }";
        }

        #endregion
        #region Extentions
        #region Math
        public static Vector3 Bezier2(Vector3 Start, Vector3 Control, Vector3 End, float t)
            => (1 - t) * (1 - t) * Start + 2 * t * (1 - t) * Control + t * t * End;
        public static Vector3 Bezier3(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end, float t)
        {
            var tm = 1 - t;
            return tm * tm * tm * start + 3 * t * tm * tm * control1 + 3 * t * t * tm * control2 + t * t * t * end;
        }

        public static T Clamp<T>(this T target, T min, T max) where T : IComparable
        {
            if (target.CompareTo(min) < 0) return min;
            if (target.CompareTo(max) > 0) return max;
            return target;
        }
        #endregion
        #region Vector
        public static Vector2Int GetDelta(this Vector2Int vect, int dx, int dy) => new (vect.x + dx, vect.y + dy);
        public static Vector2Int GetDelta(this Vector2Int vect, Vector2Int d) => GetDelta(vect, d.x, d.y);

        public static Vector3 toVector3(this Vector2 vec) => new (vec.x, vec.y);
        public static Vector3 ToVector3(this Vector2Int vec, int z = 0) => new (vec.x, vec.y, z);
        public static Vector3 toVector3(this float f) => new (f, f, f);
        public static Vector3Int ToVector3Int(this Vector2Int vec, int z = 0) => new (vec.x, vec.y, z);
        public static Vector2 toVector2(this Vector3 vec) => new (vec.x, vec.y);

        public static Vector3 Clamp(this Vector3 vector, Vector3 minBounds, Vector3 maxBounds)
        {
            var x = Mathf.Clamp(vector.x, minBounds.x, maxBounds.x);
            var y = Mathf.Clamp(vector.y, minBounds.y, maxBounds.y);
            var z = Mathf.Clamp(vector.z, minBounds.z, maxBounds.z);
            return new Vector3(x, y, z);
        }
        public static Vector2 Clamp(this Vector2 vector, Vector2 minBounds, Vector2 maxBounds)
        {
            var x = Mathf.Clamp(vector.x, minBounds.x, maxBounds.x);
            var y = Mathf.Clamp(vector.y, minBounds.y, maxBounds.y);
            return new Vector2(x, y);
        }

        public static Vector3 Set(this Vector3 v, float? x = null, float? y = null, float? z = null) =>
            new (x ?? v.x, y ?? v.y, z ?? v.z);
        public static Vector2 Set(this Vector2 v, float? x = null, float? y = null) =>
            new (x ?? v.x, y ?? v.y);

        public static Vector3 Add(this Vector3 v, float? x = null, float? y = null, float? z = null) =>
            new (v.x + (x ?? 0), v.y + (y ?? 0), v.z + (z ?? 0));
        public static Vector2 Add(this Vector2 v, float? x = null, float? y = null) =>
            new (v.x + (x ?? 0), v.y + (y ?? 0));

        public static Vector2 MultiplyElementWise(this Vector2 v, float? x = null, float? y = null, float? z = null) =>
            new(x == null ? v.x : v.x * x.Value,
                y == null ? v.y : v.y * y.Value);
        public static Vector3 MultiplyElementWise(this Vector3 v, float? x = null, float? y = null, float? z = null) =>
            new(x == null ? v.x : v.x * x.Value,
                y == null ? v.y : v.y * y.Value,
                z == null ? v.z : v.z * z.Value);
        #endregion
        #region Color
        public static Color Set(this Color col, float? r = null, float? g = null, float? b = null, float? a = null) =>
            new(r ?? col.r, g ?? col.g, b ?? col.b, a ?? col.a);
        public static Color SetAlpha(this Color color, float alpha) =>
            new(color.r, color.g, color.b, alpha);

        public static Image SetColor(this Image i, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            i.color = i.color.Set(r, g, b, a);
            return i;
        }
        public static TextMeshProUGUI SetColor(this TextMeshProUGUI i, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            i.color = i.color.Set(r, g, b, a);
            return i;
        }
        public static SpriteRenderer SetColor(this SpriteRenderer i, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            i.color = i.color.Set(r, g, b, a);
            return i;
        }

        public static Color NumberToColor(this int number) => new((number >> 24) % 256, (number >> 16) % 256, (number >> 8) % 256, number % 256);
        public static Color HexToColor(string hexColor) => Convert.ToInt32(hexColor, 16).NumberToColor();
        public static string ColorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }
        #endregion

        public static void AddSingle<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        public static void DestroyAllChildren(this Transform transform, params Transform[] exclusions)
        {
            transform.GetChildren().Where(child => exclusions.Contains(child) == false).ToList().ForEach(child => GameObject.Destroy(child.gameObject));
        }

        #region Unity hierarchy utils
        
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (!gameObject) return null;

            var t = gameObject.GetComponent<T>();
            if (!t) t = gameObject.AddComponent<T>();
            return t;
        }

        public static List<Transform> GetChildren(this Transform t)
        {
            var r = new List<Transform>();
            for (var i = 0; i < t.childCount; i++) r.Add(t.GetChild(i));
            return r;
        }

        public static void ForeachChildrenRecursively(this Transform t, Action<Transform> action)
        {
            if (t == null) return;
            action(t);
            for (var i = 0; i < t.childCount; i++)
                t.GetChild(i).ForeachChildrenRecursively(action);
        }

        #endregion

        #region UI
    
        public static void Show(this CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        public static void Hide(this CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    
        #endregion
        #endregion
    }
}