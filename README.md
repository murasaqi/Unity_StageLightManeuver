# Unity_StageLightManeuver

## 概要
UnityのTimeline上で直感的に照明演出を設計することができるアセットです。
DMXやArtnetで制御される照明コントロールシステムを参考に開発されており、実際のライブ照明に近い演出をつけることが可能です。

![Overview](overview.gif)

## インストール
#### Git URLを使用してインストール
1. `Window` > `Package Manager` を開きます。
2. `+` > `Add package from git URL...` を選択します。
3. ```https://github.com/murasaqi/Unity_StageLightManeuver.git?path=/jp.iridescenet.stagelightmaneuver``` を入力し、`Add` を押すことで最新版がインストールされます。

## 要件

- Unity 2021.3.6f1 以上
- URP or HDRP
#### Optional
- [Volumetric Light Beam](https://assetstore.unity.com/packages/vfx/shaders/volumetric-light-beam-99888)の操作にも対応しています。
Volumetric Light Beamと連携させるためには別途asmdefの設定が必要です。
詳細については [Volumetric Light Beamとの連携]() を参照してください

## 機能例

- Timelineから複数灯体の一括制御が可能
    - 灯体を役割単位でグループ化することで、大量のライトを一元管理しながら効率的にライト演出を作成できます。
    - グループ化された灯体は、灯体同士の演出ディレイやイージングを簡単に適用できるように設計されているので、1グループにつきタイムライントラック1つで演出をつけることができます。

![20240201](20240201.gif)

- TimelineClipからチャンネルに応じたプロパティを設定することで、効率的に照明演出を設計できます
    - Intensity, Color, Pan/Tilt, Gobo をはじめとした 20以上のプロパティが用意されています
    - 各チャンネル, プロパティは単純な機能しか持ちませんが、これらを組み合わせることで複雑な演出を作成することができます
    - クリップをブレンドすることで演出同士を自然に遷移することも可能です
    
    ![propDiff](prop_diff.png)
    
    ![Light, Pan/Tilt Property 単体の動作と組み合わせた際の動作](props.gif)
    
    Light, Pan/Tilt Property 単体の動作と組み合わせた際の動作
    
    ![クリップのブレンドによるライト消灯](clip_blend.gif)
    
    クリップのブレンドによるライト消灯
    
- TimelineClipをProfileとして保存し、クリップ間でパラメータの同期が可能です
    - 演出をプロファイルにすることで、変更や再利用が容易になります
- [VolumetricLightBeam](https://assetstore.unity.com/packages/vfx/shaders/volumetric-light-beam-99888)と連携させることで、軽量なボリューメトリックライトを演出に組み込むことができます
    
    ![link_vlb](link_vlb.gif)
    

## プリセット灯体

`/Resources/SLSAssets/Lights/` 以下に URP/HDRP対応のセットアップ済み灯体を5つ同梱しています

| Light Type | File Name | RenderPipeLine | Rendering Image |
|:-----------|:----------|:--------------:|:---------------:|
| LED Strobe | `SLM_LEDStrobe.prefab` | URP | <img src="SLM_LEDStrobe.png" width="64"> |
| Moving Beam Light | `SLM_MovingBeamLight_HDRP.prefab` | HDRP | <img src="SLM_MovingBeamLight_HDRP.png" width="64"> |
| Moving Beam Light | `SLM_MovingBeamLight_URP_HD.prefab` | URP | <img src="SLM_MovingBeamLight_URP_HD.png" width="64"> |
| Moving Beam Light | `SLM_MovingBeamLight_URP_SD.prefab` | URP | <img src="SLM_MovingBeamLight_URP_SD.png" width="64"> |
| Moving Wash Light | `SLM_MovingWashLight_URl.prefab` | URP | <img src="SLM_MovingWashLight_URp.png" width="64"> |
| Rotating Wash Light | `SLM_RotatingWashLight_URP.prefab` | URP | <img src="SLM_RotatingWashLight_URP.png" width="64"> |

## 使い方

### **クイックスタート**

以下にプリセット灯体を用いたタイムラインからの灯体制御例を示します。
この例ではURP上でMoving Beam Lightを扱います。

1. シーンに `/Resources/SLSAssets/Lights/SLM_MovingBeamLight_URP_HD.prefab` を配置し、必要に応じてLightFixtureと各LightChannelの設定を変更します
    1. 灯体がどんな機能に対応しているかは Stage Light Fixture によって管理されます
    2. Stage Light Fixture に機能を登録するにはFixtureコンポーネントの `Add New Channel`から追加したい機能を選択します
    
    ![fixture_ui](fixture_ui.png)
    
2. 1で作成した灯体を複製し配置した後、それらを Stage Light Universe コンポーネントをもつゲームオブジェクトの子にします
3. Stage Light Universe コンポーネントのコンテキストメニューから「**Find Stage Light Fixtures**」と「**Initialize**」を実行してください。これで灯体をタイムラインから操作できるようになります。
4. タイムラインにStage Light Timeline Trackを作成し、トラックにStage Light Universeをバインドします
5. トラックにクリップを作成すると灯体の対応チャンネルに応じて自動的にプロパティが追加されます
6. クリップからプロパティの設定を変更すると、それに応じて灯体が制御されます
    
    [tutorial](tutorial.mp4)
    

### **Volumetric Light Beamとの連携**

本パッケージはVolumetric Light Beam(以下VLB)と連携することができます。
以下の手順でVLBをパッケージとして取り込むことで、VLB付き灯体をStage Light Maneuverで制御できるようになります。

1. アセットストアからVLBをインストール後 VLBのフォルダに`com.saladgamer.volumetriclightbeam`という名前の Assembly Definition を作成してください
    1. URPを使用する場合、Assembly Definition References に`Unity.RenderPipelines.Universal.Runtime` を設定してください
    2. プラットフォーム設定はAnyに設定します

![vlb_asmdef](vlb_asmdef.png)

![vlb_asmdef_refurp](vlb_asmdef_refurp.png)

1. Assembly Definition の設置後、VLBのフォルダをPackagesフォルダ以下に移動してください
    
    ![move_vlb](move_vlb.png)
    
3. 以降VLBがついた灯体をStage Light Maneuverで制御すると、自動的にVLBのパラメータも更新されるようになります。

## コントリビューション

TRIBALCON inc
Compositon inc
[Murasaqi](https://github.com/murasaqi)
[clocknote.](https://github.com/clocknote)
[Kuyuri Iroha](https://github.com/kuyuri-iroha)
[pon](https://github.com/AJpon)

## ライセンス

MIT License
Copyright (c) 2024 Murasaqi
