using Newtonsoft.Json;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Particles;
using TheForestWaiter.Shared;

namespace TheForestWaiter.Content
{
	class ParticleCache : ContentCache<ParticleProp>
	{
		public ParticleCache(ContentConfig config) : base(config)
		{

		}

		protected override ContentType Type => ContentType.Particle;

		protected override ParticleProp LoadFromBytes(byte[] bytes)
		{
			var json = string.Concat(Encoding.UTF8.GetString(bytes).Skip(1));
			return JsonConvert.DeserializeObject<ParticleProp>(json);
		}

		public override ParticleProp Get(string name)
		{
			return (ParticleProp)base.Get(name).Clone();
		}

		public ParticleProp Get(string name, Vector2f at)
		{
			var prop = Get(name);
			prop.Position = at;
			return prop;
		}

		/// <summary>
		/// Angle in radians
		/// </summary>
		public ParticleProp Get(string name, Vector2f at, float angle, float speed)
		{
			var prop = Get(name, at);
			prop.Velocity = TrigHelper.FromAngleRad(angle, speed);
			return prop;
		}
	}
}
