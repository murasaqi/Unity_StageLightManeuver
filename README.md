# Unity StageLightManeuver

> [!Note]
> インストール用URLはこちら
> ```
> https://github.com/murasaqi/Unity_StageLightManeuver.git?path=/jp.iridescenet.stagelightmaneuver
> ```


## 概要
UnityのTimeline上で直感的に照明演出を設計することができるアセットです。
DMXやArtnetで制御される照明コントロールシステムを参考に開発されており、実際のライブ照明に近い演出をつけることが可能です。

![Overview](https://github.com/murasaqi/Unity_StageLightManeuver/assets/49616225/69f82e8a-3115-4442-8db2-5d69b145a264)

## インストール
#### Git URLを使用してインストール
1. `Window` > `Package Manager` を開きます。
2. `+` > `Add package from git URL...` を選択します。
3. ```https://github.com/murasaqi/Unity_StageLightManeuver.git?path=/jp.iridescenet.stagelightmaneuver``` を入力し、`Add` を押すことで最新版がインストールされます。

## 要件
* Unity 2021.3.6f1 以上
* URP or HDRP
#### Optional
* [Volumetric Light Beam](https://assetstore.unity.com/packages/vfx/shaders/volumetric-light-beam-99888)の制御に対応しています。
    * Volumetric Light Beamと連携するには別途Assembly Definitionの設定が必要です。
    * 詳細については [Volumetric Light Beamとの連携](#volumetric-light-beamとの連携) を参照してください

## 機能例

- Timelineから複数灯体の一括制御が可能
    - 灯体を役割単位でグループ化することで、大量のライトを一元管理しながら効率的にライト演出を作成できます。
    - グループ化された灯体は、灯体同士の演出ディレイやイージングを簡単に適用できるように設計されているので、1グループにつきタイムライントラック1つで演出をつけることができます。

    ![feature_overview](https://github.com/murasaqi/Unity_StageLightManeuver/assets/49616225/f3426147-89bc-4f57-a68b-8e6f8d9499b0)


- TimelineClipからチャンネルに応じたプロパティを設定することで、効率的に照明演出を設計できます
    - Intensity, Color, Pan/Tilt, Gobo をはじめとした 20以上のプロパティが用意されています
    - 各チャンネル, プロパティは単純な機能しか持ちませんが、これらを組み合わせることで複雑な演出を作成することができます
    - クリップをブレンドすることで演出同士を自然に遷移することも可能です
    
        ![property](https://github.com/murasaqi/Unity_StageLightManeuver/assets/49616225/90de445d-baab-45a7-bcf8-d9681a950523)
        > Light, Pan/Tilt Property 単体の動作と組み合わせた際の動作
        
        ![clip_blend](https://github.com/murasaqi/Unity_StageLightManeuver/assets/49616225/690e6683-dece-4a5e-b7f5-9ea2b7462cfa)
        > クリップのブレンドによるライト消灯


- TimelineClipをProfileとして保存し、クリップ間でパラメータの同期が可能です
    - 演出をプロファイルにすることで、変更や再利用が容易になります
- [VolumetricLightBeam](https://assetstore.unity.com/packages/vfx/shaders/volumetric-light-beam-99888)と連携させることで、軽量なボリューメトリックライトを演出に組み込むことができます
    ![link_vlb](https://github.com/murasaqi/Unity_StageLightManeuver/assets/49616225/b3ada0a7-3cc6-4b11-87a0-d719b5d7e266)
    

## プリセット灯体

`/Resources/SLSAssets/Lights/` 以下に URP/HDRP対応のセットアップ済み灯体を5つ同梱しています

| Light Type | File Name | RenderPipeLine | Rendering Image |
|:-----------|:----------|:--------------:|:---------------:|
| LED Strobe | `SLM_LEDStrobe.prefab` | URP | <img src="https://github.com/murasaqi/Unity_StageLightManeuver/assets/49616225/c3df799b-4bd5-484b-a7c0-0a35c22a008a" width="64"> |
| Moving Beam Light | `SLM_MovingBeamLight_HDRP.prefab` | HDRP | <img src="https://github.com/murasaqi/Unity_StageLightManeuver/assets/49616225/009226e2-5918-4609-9cbc-82bc733783c2" width="64"> |
| Moving Beam Light | `SLM_MovingBeamLight_URP_HD.prefab` | URP | <img src="https://github.com/murasaqi/Unity_StageLightManeuver/assets/49616225/2f0b3b68-dda4-4a40-b3a4-1f33cb2f0b96" width="64"> |
| Moving Beam Light | `SLM_MovingBeamLight_URP_SD.prefab` | URP | <img src="https://github.com/murasaqi/Unity_StageLightManeuver/assets/49616225/2f0b3b68-dda4-4a40-b3a4-1f33cb2f0b96" width="64"> |
| Moving Wash Light | `SLM_MovingWashLight_URl.prefab` | URP | <img src="https://github.com/murasaqi/Unity_StageLightManeuver/assets/49616225/d4e6141c-5407-4c85-8fd4-371b039ced32" width="64"> |
| Rotating Wash Light | `SLM_RotatingWashLight_URP.prefab` | URP | <img src="https://github.com/murasaqi/Unity_StageLightManeuver/assets/49616225/6fca80dc-6d36-42de-8a76-15c92b396573" width="64"> |

## 使い方

### **クイックスタート**
 [tutorial](https://github.com/murasaqi/Unity_StageLightManeuver/assets/49616225/acd11990-80a2-4ea1-ba1c-964e0ed2a19a)
 
以下にプリセット灯体を用いたタイムラインからの灯体制御例を示します。
この例ではURP上でMoving Beam Lightを扱います。

1. シーンに `/Resources/SLSAssets/Lights/SLM_MovingBeamLight_URP_HD.prefab` を配置し、必要に応じてLightFixtureと各LightChannelの設定を変更します
    * 灯体がどんな機能に対応しているかは Stage Light Fixture によって管理されます
    * Stage Light Fixture に機能を登録するにはFixtureコンポーネントの `Add New Channel`から追加したい機能を選択します
          <blockquote><details><summary>参考</summary>
              ![fixture_ui](https://github.com/murasaqi/Unity_StageLightManeuver/assets/49616225/a1569ee6-a03a-4816-a3c3-6e57f765664f)
          </details></blockquote>
    
2. 1で作成した灯体を複製し配置した後、それらを Stage Light Universe コンポーネントをもつゲームオブジェクトの子にします
3. Stage Light Universe コンポーネントのコンテキストメニューから「**Find Stage Light Fixtures**」と「**Initialize**」を実行してください。これで灯体をタイムラインから操作できるようになります。
4. タイムラインにStage Light Timeline Trackを作成し、トラックにStage Light Universeをバインドします
5. トラックにクリップを作成すると灯体の対応チャンネルに応じて自動的にプロパティが追加されます
6. クリップからプロパティの設定を変更すると、それに応じて灯体が制御されます
    

### **Volumetric Light Beam**
ムービングライトの演出を作るうえでボリューメトリックライトの存在は欠かせません。
ボリューメトリックライトを使うことでムービングライトの光が空間全体に広がり、よりリアルな演出を作ることができます。/
<!-- VLB有り無しの画像 or GIF -->

しかし標準でボリューメトリックライトをサポートしているのはHDRPのみで、URPではサポートされていません。
またHDRPのボリューメトリックライトは高負荷であるため、大量に使うことはできません。

これらの問題に対処するために、Stage Light Maneuver では Volumetric Light Beam を利用することを推奨します。

Volumetric Light Beam はURP、HDRPの両方で使用可能な、軽量で高品質なボリューメトリックライトエフェクトを提供します。
StageLightManeunver は Volumetric Light Beam を使ったボリューメトリックライトの演出設計に対応しているので、ぜひご利用ください。


### **Volumetric Light Beamとの連携**

本パッケージはVolumetric Light Beam(以下VLB)との連携にも対応しています。
以下の手順でVLBをパッケージとして取り込むことで、VLB付き灯体をStage Light Maneuverで制御できるようになります。

1. アセットストアからVLBをインストール後 VLBのフォルダに`com.saladgamer.volumetriclightbeam`という名前の Assembly Definition を作成してください
    * プラットフォーム設定はAnyに設定します
    * URPを使用する場合、`com.saladgamer.volumetriclightbeam`ファイルの設定内にあるAssembly Definition References に`Unity.RenderPipelines.Universal.Runtime` を設定してください
                       ![vlb_asmdef_refurp](https://github.com/murasaqi/Unity_StageLightManeuver/assets/49616225/73a2ae12-f58a-422d-9290-47e82f9a9f8d)
        
    ![vlb_asmdef](https://github.com/murasaqi/Unity_StageLightManeuver/assets/49616225/92ee03d3-1b7a-4f88-a50e-363dfd3a6dc6)

2. Assembly Definition の設置後、VLBのフォルダをPackagesフォルダ以下に移動してください
    
    ![move_vlb](https://github.com/murasaqi/Unity_StageLightManeuver/assets/49616225/e4c89bea-7b2e-486f-9f29-bf92387cc524)
    
3. 以降VLBがついた灯体をStage Light Maneuverで制御すると、自動的にVLBのパラメータも更新されるようになります。

## Contributor

TRIBALCON inc  
Compositon inc  
[Murasaqi](https://github.com/murasaqi)  
[clocknote.](https://github.com/clocknote)  
[Kuyuri Iroha](https://github.com/kuyuri-iroha)  
[pon](https://github.com/AJpon)  

## License

MIT License
Copyright (c) 2024 Murasaqi
