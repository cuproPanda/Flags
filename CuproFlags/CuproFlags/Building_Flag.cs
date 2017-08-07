using System.IO;

using UnityEngine;
using Verse;

namespace CuproFlags {
	[StaticConstructorOnStartup]
	public class Building_Flag : Building {

		public static Graphic Pole = GraphicDatabase.Get<Graphic_Single>("Cupro/Object/Utility/Flags/Pole", ShaderDatabase.CutoutComplex, new Vector2(3, 3), Color.white, Color.white);

		private Graphic[] animFrames;
		private Graphic currFrame;
		private int frameLerp = 0;
		private string wd;
		private bool cutout;
		private bool updated;
		private int swayTicks = 10;

		public override Graphic Graphic {
			get {
				if (updated) {
					return Pole;
				}
				return base.Graphic;
			}
		}

		private Color Color {
			get {
				if (Stuff != null) {
					return Stuff.stuffProps.color;
				}
				return def.graphicData.color;
			}
		}


		public override void SpawnSetup(Map map, bool respawningAfterLoad) {
			base.SpawnSetup(map, respawningAfterLoad);
			wd = Path.GetDirectoryName(def.graphicData.texPath);
			LongEventHandler.ExecuteWhenFinished(GetGraphicArray);
		}


		public void GetGraphicArray() {
			cutout = def.graphicData.shaderType == ShaderType.CutoutComplex;
			animFrames = new Graphic_Single[5];
			if (cutout) {
				for (int i = 0; i < 5; i++) {
					animFrames[i] = GraphicDatabase.Get<Graphic_Single>($"{wd}/Anim{i}", ShaderDatabase.CutoutComplex, new Vector2(3, 3), Color.white);
				}
			}
			else {
				for (int i = 0; i < 5; i++) {
					animFrames[i] = GraphicDatabase.Get<Graphic_Single>($"{wd}/Anim{i}", ShaderDatabase.DefaultShader, new Vector2(3, 3), Color.white);
				}
			}
			
			currFrame = animFrames[Rand.Range(0,5)];
			updated = true;
		}


		public override void Tick() {
			base.Tick();

			if (this.IsHashIntervalTick(swayTicks)) {
				frameLerp++;
				if (frameLerp >= 5) {
					frameLerp = 0;
				}
				currFrame = animFrames[frameLerp];
			}

			if (this.IsHashIntervalTick(60)) {
				swayTicks = (int)GenMath.LerpDouble(0.3f, 1.5f, 15f, 5f, Map.windManager.WindSpeed);
			}
		}


		public override void Draw() {
			base.Draw();
			if (currFrame != null) {
				Matrix4x4 matrix = default(Matrix4x4);
				Vector3 s = new Vector3(def.graphicData.drawSize.x, 9f, def.graphicData.drawSize.y);
				Vector3 x = new Vector3(0f, 0f, 0f);
				matrix.SetTRS(DrawPos + x + Altitudes.AltIncVect, Rotation.AsQuat, s);
				Graphics.DrawMesh(MeshPool.plane10, matrix, currFrame.GetColoredVersion(ShaderDatabase.ShaderFromType(def.graphicData.shaderType), Color, Color.white).MatSingle, 0);
			}
		}
	}
}
