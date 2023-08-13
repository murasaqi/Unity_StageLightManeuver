using UnityEngine;

namespace StageLightManeuver
{
	public static class EaseUtil
	{
		public static float EaseLinear(float t, float total, float begin, float end)
		{
			return (end - begin) * t / total + begin;
			
			//(1 - -1) * 0.5/1 + 1
		}

		#region Quad

		public static float EaseInQuad(float t, float total, float begin, float end)
		{
			t /= total;
			return (end - begin) * t * t + begin;
		}

		public static float EaseOutQuad(float t, float total, float begin, float end)
		{
			t /= total;
			return -(end - begin) * t * (t - 2f) + begin;
		}

		public static float EaseInOutQuad(float t, float total, float begin, float end)
		{
			t /= total / 2;
			if (t < 1)
				return (end - begin) / 2 * t * t + begin;
			--t;
			return -(end - begin) / 2 * (t * (t - 2) - 1) + begin;
		}

		#endregion

		#region Cubic

		public static float EaseInCubic(float t, float total, float begin, float end)
		{
			t /= total;
			return (end - begin) * t * t * t + begin;
		}

		public static float EaseOutCubic(float t, float total, float begin, float end)
		{
			t = t / total - 1f;
			return (end - begin) * (t * t * t + 1f) + begin;
		}

		public static float EaseInOutCubic(float t, float total, float begin, float end)
		{
			t /= total / 2;
			if (t < 1f)
				return (end - begin) / 2f * t * t * t + begin;
			t -= 2f;
			return (end - begin) / 2f * (t * t * t + 2f) + begin;
		}

		#endregion

		#region Quart

		public static float EaseInQuart(float t, float total, float begin, float end)
		{
			t /= total;
			return (end - begin) * t * t * t * t + begin;
		}

		public static float EaseOutQuart(float t, float total, float begin, float end)
		{
			t = t / total - 1f;
			return -(end - begin) * (t * t * t * t - 1f) + begin;
		}

		public static float EaseInOutQuart(float t, float total, float begin, float end)
		{
			t /= total / 2;
			if (t < 1f)
				return (end - begin) / 2f * t * t * t * t + begin;
			t -= 2f;
			return -(end - begin) / 2f * (t * t * t * t - 2f) + begin;
		}

		#endregion

		#region Quint

		public static float EaseInQuint(float t, float total, float begin, float end)
		{
			t /= total;
			return (end - begin) * t * t * t * t * t + begin;
		}

		public static float EaseOutQuint(float t, float total, float begin, float end)
		{
			t = t / total - 1f;
			return (end - begin) * (t * t * t * t * t + 1f) + begin;
		}

		public static float EaseInOutQuint(float t, float total, float begin, float end)
		{
			t /= total / 2;
			if (t < 1f)
				return (end - begin) / 2f * t * t * t * t * t + begin;
			t -= 2f;
			return (end - begin) / 2f * (t * t * t * t * t + 2f) + begin;
		}

		#endregion

		#region Sine

		public static float EaseInSine(float t, float total, float begin, float end)
		{
			return -(end - begin) * Mathf.Cos(t / total * (Mathf.PI / 2.0f)) + (end - begin) + begin;
		}

		public static float EaseOutSine(float t, float total, float begin, float end)
		{
			return (end - begin) * Mathf.Sin(t / total * (Mathf.PI / 2.0f)) + begin;
		}

		public static float EaseInOutSine(float t, float total, float begin, float end)
		{
			return -(end - begin) / 2.0f * (Mathf.Cos(Mathf.PI * t / total) - 1) + begin;
		}

		#endregion

		#region Expo

		public static float EaseInExpo(float t, float total, float begin, float end)
		{
			if (t <= 0f)
				return begin;
			return (end - begin) * Mathf.Pow(2f, 10f * (t / total - 1f)) + begin;
		}

		public static float EaseOutExpo(float t, float total, float begin, float end)
		{
			if (t >= total)
				return end;
			return (end - begin) * (-Mathf.Pow(2f, -10f * t / total) + 1f) + begin;
		}

		public static float EaseInOutExpo(float t, float total, float begin, float end)
		{
			if (t <= 0f)
				return begin;
			if (t >= total)
				return end;
			t /= total / 2;
			if (t < 1f)
				return (end - begin) / 2 * Mathf.Pow(2f, 10f * (t - 1f)) + begin;
			--t;
			return (end - begin) / 2 * (-Mathf.Pow(2f, -10f * t) + 2f) + begin;
		}

		#endregion

		#region Circ

		public static float EaseInCirc(float t, float total, float begin, float end)
		{
			t /= total;
			return -(end - begin) * (Mathf.Sqrt(1f - t * t) - 1f) + begin;
		}

		public static float EaseOutCirc(float t, float total, float begin, float end)
		{
			t = t / total - 1f;
			return (end - begin) * Mathf.Sqrt(1f - t * t) + begin;
		}

		public static float EaseInOutCirc(float t, float total, float begin, float end)
		{
			t /= total / 2;
			if (t < 1f)
				return -(end - begin) / 2f * (Mathf.Sqrt(1f - t * t) - 1f) + begin;
			t -= 2f;
			return (end - begin) / 2f * (Mathf.Sqrt(1f - t * t) + 1f) + begin;
		}

		#endregion

		#region Back

		public static float EaseInBack(float t, float total, float begin, float end, float s = 1.70158f)
		{
			t /= total;
			return (end - begin) * t * t * ((s + 1f) * t - s) + begin;
		}

		public static float EaseOutBack(float t, float total, float begin, float end, float s = 1.70158f)
		{
			t = t / total - 1f;
			return (end - begin) * (t * t * ((s + 1f) * t + s) + 1f) + begin;
		}

		public static float EaseInOutBack(float t, float total, float begin, float end, float s = 1.70158f)
		{
			t /= total / 2f;
			s *= 1.525f;
			if (t < 1f)
				return (end - begin) / 2f * (t * t * ((s + 1f) * t - s)) + begin;
			t -= 2f;
			return (end - begin) / 2f * (t * t * ((s + 1f) * t + s) + 2f) + begin;
		}

		#endregion

		#region Bounce

		public static float EaseOutBounce(float t, float total, float begin, float end)
		{
			t /= total;
			if (t < 1.0f / 2.75f)
				return (end - begin) * (7.5625f * t * t) + begin;
			else if (t < 2.0f / 2.75f)
				return (end - begin) * (7.5625f * (t -= 1.5f / 2.75f) * t + 0.75f) + begin;
			else if (t < 2.5f / 2.75f)
				return (end - begin) * (7.5625f * (t -= 2.25f / 2.75f) * t + 0.9375f) + begin;
			return (end - begin) * (7.5625f * (t -= 2.625f / 2.75f) * t + 0.984375f) + begin;
		}

		public static float EaseInBounce(float t, float total, float begin, float end)
		{
			return end - begin - EaseOutBounce(total - t, total, 0, end - begin) + begin;
		}

		public static float EaseInOutBounce(float t, float total, float begin, float end)
		{
			if (t < total / 2f)
				return EaseInBounce(t * 2f, total, 0f, end - begin) * 0.5f + begin;
			return EaseOutBounce(t * 2f - total, total, 0f, end - begin) * 0.5f + begin + (end - begin) * 0.5f;
		}

		#endregion

		#region Elastic

		public static float EaseInElastic(float t, float total, float begin, float end, float a = 0f, float p = 0f)
		{
			if (t == 0f)
				return begin;
			t /= total;
			if (t == 1f)
				return begin + (end - begin);
			if (p == 0f)
				p = total * 0.3f;
			float s;
			if (a == 0f || a < Mathf.Abs(end - begin))
			{
				a = end - begin;
				s = p / 4.0f;
			}
			else
			{
				s = p / Mathf.PI * 2f * Mathf.Asin((end - begin) / a);
			}

			t -= 1;
			return -(a * Mathf.Pow(2f, 10f * t) * Mathf.Sin((t * total - s) * Mathf.PI * 2f / p)) + begin;
		}

		public static float EaseOutElastic(float t, float total, float begin, float end, float a = 0, float p = 0)
		{
			if (t == 0f)
				return begin;
			t /= total;
			if (t == 1f)
				return begin + (end - begin);
			if (p == 0f)
				p = total * 0.3f;
			float s;
			if (a == 0f || a < Mathf.Abs(end - begin))
			{
				a = end - begin;
				s = p / 4.0f;
			}
			else
			{
				s = p / Mathf.PI * 2f * Mathf.Asin((end - begin) / a);
			}

			return a * Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * total - s) * Mathf.PI * 2f / p) + (end - begin) + begin;
		}
		

		public static float EaseInOutElastic(float t, float total, float begin, float end, float a = 0, float p = 0)
		{
			if (t == 0f)
				return begin;
			t /= total / 2;
			if (t == 2f)
				return begin + (end - begin);
			if (p == 0f)
				p = total * (0.3f * 1.5f);
			float s;
			if (a == 0f || a < Mathf.Abs(end - begin))
			{
				a = end - begin;
				s = p / 4.0f;
			}
			else
			{
				s = p / Mathf.PI * 2f * Mathf.Asin((end - begin) / a);
			}

			if (t < 1f)
			{
				t -= 1f;
				return -0.5f * (a * Mathf.Pow(2f, 10f * t) * Mathf.Sin((t * total - s) * Mathf.PI * 2f / p)) + begin;
			}

			t -= 1f;
			return a * Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * total - s) * Mathf.PI * 2f / p) * 0.5f + (end - begin) +
			       begin;
		}

		#endregion


		public static float GetEaseValue(EaseType easeType, float t, float total, float begin, float end, float a = 0,
			float p = 0)
		{
			var result = 0f;
				switch (easeType)
                {
                    case EaseType.Linear:
                        result = EaseUtil.EaseLinear(t,total,begin,end);
                        break;
                    
                    case EaseType.InQuad:
                        result = EaseUtil.EaseInQuad(t,total,begin,end);
                        break;
                    case EaseType.OutQuad:
                        result = EaseUtil.EaseOutQuad(t,total,begin,end);
                        break;
                    case EaseType.InOutQuad:
                        result = EaseUtil.EaseInOutQuad(t,total,begin,end);
                        break;
                    
                    case EaseType.InCubic:
                        result = EaseUtil.EaseInCubic(t,total,begin,end);
                        break;
                    case EaseType.OutCubic:
                        result = EaseUtil.EaseOutCubic(t,total,begin,end);
                        break;
                    case EaseType.InOutCubic:
                        result = EaseUtil.EaseInOutCubic(t,total,begin,end);
                        break;
                    
                    case EaseType.InQuart:
                        result = EaseUtil.EaseInQuart(t,total,begin,end);
                        break;
                    case EaseType.OutQuart:
                        result = EaseUtil.EaseOutQuart(t,total,begin,end);
                        break;
                    case EaseType.InOutQuart:
                        result = EaseUtil.EaseInOutQuart(t,total,begin,end);
                        break;
                    
                    case EaseType.InQuint:
                        result = EaseUtil.EaseInQuint(t,total,begin,end);
                        break;
                    case EaseType.OutQuint:
                        result = EaseUtil.EaseOutQuint(t,total,begin,end);
                        break;
                    case EaseType.InOutQuint:
                        result = EaseUtil.EaseInOutQuint(t,total,begin,end);
                        break;
                    
                    case EaseType.InSine:
                        result = EaseUtil.EaseInSine(t,total,begin,end);
                        break;
                    case EaseType.OutSine:
                        result = EaseUtil.EaseOutSine(t,total,begin,end);
                        break;
                    case EaseType.InOutSine:
                        result = EaseUtil.EaseInOutSine(t,total,begin,end);
                        break;

                    case EaseType.InExpo:
                        result = EaseUtil.EaseInExpo(t,total,begin,end);
                        break;
                    case EaseType.OutExpo:
                        result = EaseUtil.EaseOutExpo(t,total,begin,end);
                        break;
                    case EaseType.InOutExpo:
                        result = EaseUtil.EaseInOutExpo(t,total,begin,end);
                        break;
                    
                    case EaseType.InCirc:
                        result = EaseUtil.EaseInCirc(t,total,begin,end);
                        break;
                    case EaseType.OutCirc:
                        result = EaseUtil.EaseOutCirc(t,total,begin,end);
                        break;
                    case EaseType.InOutCirc:
                        result = EaseUtil.EaseInOutCirc(t,total,begin,end);
                        break;
                    
                    case EaseType.InBack:
                        result = EaseUtil.EaseInBack(t,total,begin,end);
                        break;
                    case EaseType.OutBack:
                        result = EaseUtil.EaseOutBack(t,total,begin,end);
                        break;
                    case EaseType.InOutBack:
                        result = EaseUtil.EaseInOutBack(t,total,begin,end);
                        break;
                    
                    case EaseType.InBounce:
                        result = EaseUtil.EaseInBounce(t,total,begin,end);
                        break;
                    case EaseType.OutBounce:
                        result = EaseUtil.EaseOutBounce(t,total,begin,end);
                        break;
                    case EaseType.InOutBounce:
                        result = EaseUtil.EaseInOutBounce(t,total,begin,end);
                        break;
                    
                    case EaseType.InElastic:
                        result = EaseUtil.EaseInElastic(t,total,begin,end);
                        break;
                    case EaseType.OutElastic:
                        result = EaseUtil.EaseOutElastic(t,total,begin,end);
                        break;
                    case EaseType.InOutElastic:
                        result = EaseUtil.EaseInOutElastic(t,total,begin,end);
                        break;
                }

				return result;
		}
		
		
	}
}