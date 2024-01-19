using UnityEngine;

namespace StageLightManeuver
{
	public class Ease
	{
		private EaseType type;
		private float total;
		private float begin;
		private float end;

		public Ease(EaseType type, float total, float begin, float end)
		{
			this.type = type;
			this.total = total;
			this.begin = begin;
			this.end = end;
		}

		public float Eval(float t)
		{
			return Eval(type, t, total, begin, end);
		}

		public static float Eval(EaseType type, float t, float begin, float end)
		{
			return Eval(type, Mathf.Clamp01(t), 1f, begin, end);
		}

		public static float Eval(EaseType type, float t, float total, float begin, float end)
		{
			t = Mathf.Clamp(t, 0f, total);
			switch (type)
			{
				case EaseType.Linear:
					return EaseUtil.EaseLinear(t, total, begin, end);
				case EaseType.InSine:
					return EaseUtil.EaseInSine(t, total, begin, end);
				case EaseType.OutSine:
					return EaseUtil.EaseOutSine(t, total, begin, end);
				case EaseType.InOutSine:
					return EaseUtil.EaseInOutSine(t, total, begin, end);
				case EaseType.InQuad:
					return EaseUtil.EaseInQuad(t, total, begin, end);
				case EaseType.OutQuad:
					return EaseUtil.EaseOutQuad(t, total, begin, end);
				case EaseType.InOutQuad:
					return EaseUtil.EaseInOutQuad(t, total, begin, end);
				case EaseType.InCubic:
					return EaseUtil.EaseInCubic(t, total, begin, end);
				case EaseType.OutCubic:
					return EaseUtil.EaseOutCubic(t, total, begin, end);
				case EaseType.InOutCubic:
					return EaseUtil.EaseInOutCubic(t, total, begin, end);
				case EaseType.InQuart:
					return EaseUtil.EaseInQuart(t, total, begin, end);
				case EaseType.OutQuart:
					return EaseUtil.EaseOutQuart(t, total, begin, end);
				case EaseType.InOutQuart:
					return EaseUtil.EaseInOutQuart(t, total, begin, end);
				case EaseType.InQuint:
					return EaseUtil.EaseInQuint(t, total, begin, end);
				case EaseType.OutQuint:
					return EaseUtil.EaseOutQuint(t, total, begin, end);
				case EaseType.InOutQuint:
					return EaseUtil.EaseInOutQuint(t, total, begin, end);
				case EaseType.InExpo:
					return EaseUtil.EaseInExpo(t, total, begin, end);
				case EaseType.OutExpo:
					return EaseUtil.EaseOutExpo(t, total, begin, end);
				case EaseType.InOutExpo:
					return EaseUtil.EaseInOutExpo(t, total, begin, end);
				case EaseType.InCirc:
					return EaseUtil.EaseInCirc(t, total, begin, end);
				case EaseType.OutCirc:
					return EaseUtil.EaseOutCirc(t, total, begin, end);
				case EaseType.InOutCirc:
					return EaseUtil.EaseInOutCirc(t, total, begin, end);
				case EaseType.InBack:
					return EaseUtil.EaseInBack(t, total, begin, end);
				case EaseType.OutBack:
					return EaseUtil.EaseOutBack(t, total, begin, end);
				case EaseType.InOutBack:
					return EaseUtil.EaseInOutBack(t, total, begin, end);
				case EaseType.InBounce:
					return EaseUtil.EaseInBounce(t, total, begin, end);
				case EaseType.OutBounce:
					return EaseUtil.EaseOutBounce(t, total, begin, end);
				case EaseType.InOutBounce:
					return EaseUtil.EaseInOutBounce(t, total, begin, end);
				case EaseType.InElastic:
					return EaseUtil.EaseInElastic(t, total, begin, end);
				case EaseType.OutElastic:
					return EaseUtil.EaseOutElastic(t, total, begin, end);
				case EaseType.InOutElastic:
					return EaseUtil.EaseInOutElastic(t, total, begin, end);
			}

			return 0f;
		}
	}
}