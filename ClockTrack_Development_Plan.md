# Clock Track 機能実装 開発設計書

# **絶対に従え**

* 謙虚に振る舞え
* ユーザーから質問があった場合は、誠実に振る舞え
* ユーザーから質問があった場合は、直接的かつ具体的に回答せよ
* ユーザーから質問があって、その内容がよくわからないときは必ずよくわからないと回答せよ

## 1. 開発概要

### 1.1 目的
Timeline上で時間関連の設定（BPM等）を一元管理するClock Track機能を、既存の資産に影響を与えずに段階的に実装する。

### 1.2 基本方針
- 既存のコードベースと資産を最大限に尊重する
- 段階的な実装と検証を行い、各段階で動作確認を行う
- 拡張性を考慮した設計を行う

## 2. 開発フェーズ

### フェーズ1: 基本構造の実装（所要時間目安: 2-3日）

#### 目標
- Clock TrackとClockTimelineClipの基本クラスを実装
- 既存のTimelineシステムに統合するための最小限の変更を行う

#### タスク
1. **Clock Track基本クラスの作成**
   - `ClockTimelineTrack`クラスの実装
   - `ClockTimelineClip`クラスの実装
   - 既存の`ClockProperty`クラスとの連携

2. **Timeline統合**
   - TrackAssetとPlayableAssetの拡張
   - 基本的なPlayable生成機能の実装

3. **エディタ拡張の基本実装**
   - Clock Track用のカスタムエディタの基本実装
   - インスペクタUIの拡張

#### 検証項目
- [ ] Clock Trackを Timeline上に追加できる
- [ ] Clock Track上にClipを配置できる
- [ ] Clock Trackの基本的なプロパティ（BPM等）を編集できる
- [ ] Timeline再生時にClock Trackが正常に動作する（エラーが発生しない）

### フェーズ2: 影響メカニズムの実装（所要時間目安: 3-4日）

#### 目標
- Clock Trackから下位のStageLightTrackへの影響メカニズムを実装
- 既存のClipの動作を維持しながら、Clock Trackからの設定を適用する

#### タスク
1. **影響検出メカニズムの実装**
   - `PlayableDirectorExtensions.GetClockPropertyFromClockTrack`メソッドの実装
   - Timeline上のトラック階層関係の検出機能

2. **設定適用メカニズムの実装**
   - `StageLightTimelineMixerBehaviour`の拡張
   - `ApplyClockSettings`メソッドの実装
   - プロパティ単位での継承制御

3. **StageLightTimelineClipの拡張**
   - `AcceptClockTrackSettings`プロパティの追加
   - 既存のClipとの互換性確保

#### 検証項目
- [ ] Clock Trackが下位のStageLightTrackに影響を与える
- [ ] Clock TrackのBPM設定が下位のClipに反映される
- [ ] 既存のClipの動作が維持される（Clock Trackがない場合）
- [ ] 個別のClipでClock Trackの影響を拒否できる

### フェーズ3: エディタUI拡張（所要時間目安: 2-3日）

#### 目標
- Clock Trackとその影響関係を視覚的に表現するUIを実装
- ユーザーが直感的に操作できるインターフェースを提供

#### タスク
1. **Clock Track専用エディタの拡張**
   - `ClockTimelineTrackEditor`クラスの実装
   - 影響範囲の視覚的表示

2. **StageLightClipインスペクタの拡張**
   - Clock Track設定の受け入れ/拒否UI
   - 現在適用されているClock Track設定の表示

3. **視覚的フィードバックの実装**
   - Clock Trackの影響を受けているClipの視覚的表示
   - 警告・情報メッセージの表示

#### 検証項目
- [ ] Clock Trackの影響範囲が視覚的に表示される
- [ ] Clock Trackの設定を受け入れ/拒否するUIが機能する
- [ ] 現在適用されているClock Track設定が正しく表示される
- [ ] 視覚的フィードバックが直感的で分かりやすい

### フェーズ4: 高度な機能と最適化（所要時間目安: 3-4日）

#### 目標
- より高度な時間制御機能を実装
- パフォーマンスの最適化
- エッジケースの処理

#### タスク
1. **複数Clock Track対応**
   - 複数のClock Trackが存在する場合の優先順位付け
   - 階層的な影響関係の処理

2. **動的なBPM変更対応**
   - 再生中のBPM変更に対する適切な反応
   - テンポカーブの実装

3. **パフォーマンス最適化**
   - 不要な再計算の回避
   - キャッシュ機構の実装

4. **エッジケース対応**
   - Timeline編集中の動作安定化
   - 無効なデータに対する堅牢性の向上

#### 検証項目
- [ ] 複数のClock Trackが正しく機能する
- [ ] 再生中のBPM変更が適切に反映される
- [ ] パフォーマンスが許容範囲内である
- [ ] エッジケースでも安定して動作する

## 3. 実装詳細

### 3.1 コアクラス構造

```csharp
// Clock Track基本クラス
[TrackColor(0.5f, 0.8f, 0.8f)]
[TrackClipType(typeof(ClockTimelineClip))]
public class ClockTimelineTrack : TrackAsset
{
    public bool affectAllTracksBelow = true;
    
    // オーバーライドメソッド
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        // Mixerの作成
    }
}

// Clock Clipクラス
[Serializable]
public class ClockTimelineClip : PlayableAsset, ITimelineClipAsset
{
    public ClockProperty clockProperty = new ClockProperty();
    
    public ClipCaps clipCaps => ClipCaps.Blending;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // Playableの作成
    }
}

// Clock Behaviourクラス
public class ClockTimelineBehaviour : PlayableBehaviour
{
    public ClockProperty clockProperty;
    
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // フレーム処理
    }
}
```

### 3.2 影響メカニズム

```csharp
// 拡張メソッド
public static class PlayableDirectorExtensions
{
    public static ClockProperty GetClockPropertyFromClockTrack(this PlayableDirector director, TrackAsset currentTrack)
    {
        // 現在のトラックより上にあるClock Trackを検索
        // 見つかった場合はそのClockPropertyを返す
    }
}

// StageLightTimelineMixerBehaviourの拡張
public class StageLightTimelineMixerBehaviour : PlayableBehaviour
{
    // 既存のコード...
    
    private void ApplyClockSettings(StageLightTimelineClip clip, ClockProperty clockFromTrack)
    {
        // Clock Trackの設定を適用（既存の設定を尊重）
    }
}
```

### 3.3 エディタ拡張

```csharp
#if UNITY_EDITOR
// Clock Track用カスタムエディタ
[CustomTimelineEditor(typeof(ClockTimelineTrack))]
public class ClockTimelineTrackEditor : TrackEditor
{
    // 影響範囲の視覚的表示
}

// StageLightClip用カスタムインスペクタ
[CustomEditor(typeof(StageLightTimelineClip))]
public class StageLightTimelineClipCustomInspector : Editor
{
    // Clock Track設定の受け入れ/拒否UI
}
#endif
```

## 4. 開発スケジュール

### 週1: 基本実装
- 日1-2: フェーズ1（基本構造の実装）
- 日3-5: フェーズ2（影響メカニズムの実装）
- 日6-7: 中間テストと調整

### 週2: 拡張と最適化
- 日1-3: フェーズ3（エディタUI拡張）
- 日4-6: フェーズ4（高度な機能と最適化）
- 日7: 最終テストと調整

## 5. テスト計画

### 5.1 単体テスト
- 各クラスの基本機能テスト
- エッジケースのテスト（無効なデータ、極端な値など）

### 5.2 統合テスト
- Timeline上での動作テスト
- 既存のプロジェクトとの互換性テスト
- 複数のClock Trackを使用したテスト

### 5.3 パフォーマンステスト
- 大規模Timelineでのパフォーマンス測定
- メモリ使用量の測定

### 5.4 ユーザビリティテスト
- エディタUIの使いやすさテスト
- ワークフローの効率性テスト

## 6. 段階的実装のためのチェックポイント

### チェックポイント1: 基本機能の確認
- Clock Trackを作成し、BPM設定を含むClipを配置できる
- Timeline再生時にエラーが発生しない

### チェックポイント2: 影響メカニズムの確認
- Clock TrackのBPM設定が下位のStageLightClipに反映される
- 既存のClipの動作が維持される

### チェックポイント3: UI機能の確認
- Clock Trackの影響範囲が視覚的に表示される
- Clock Track設定の受け入れ/拒否UIが機能する

### チェックポイント4: 高度な機能の確認
- 複数のClock Trackが正しく機能する
- 再生中のBPM変更が適切に反映される
- パフォーマンスが許容範囲内である

## 7. リスクと対策

### 7.1 既存資産への影響リスク
- **リスク**: 既存のTimelineプロジェクトが正常に動作しなくなる
- **対策**: 
  - 非侵襲的な実装アプローチを採用
  - 段階的なテストと検証
  - 互換性モードの提供

### 7.2 パフォーマンスリスク
- **リスク**: 複雑なTimeline構成でパフォーマンスが低下する
- **対策**:
  - キャッシュ機構の実装
  - 不要な再計算の回避
  - パフォーマンス測定と最適化

### 7.3 ユーザビリティリスク
- **リスク**: 新機能が直感的に理解しにくい
- **対策**:
  - 視覚的フィードバックの充実
  - ヘルプメッセージの表示
  - サンプルプロジェクトの提供

## 8. ドキュメント計画

### 8.1 開発者ドキュメント
- クラス構造と設計思想
- 拡張ポイントと拡張方法
- APIリファレンス

### 8.2 ユーザードキュメント
- Clock Track機能の概要
- 基本的な使用方法
- 高度な使用例
- トラブルシューティング

## 9. 将来の拡張計画

### 9.1 短期的拡張（次期アップデート）
- マーカーベースのテンポマップ
- より詳細なビジュアライゼーション
- プリセット機能

### 9.2 中長期的拡張
- 外部同期機能（MIDI、OSCなど）
- AIによる自動BPM検出
- より高度なテンポ変化パターン

## 10. 結論

本設計書に基づいて段階的に実装を進めることで、既存の資産に影響を与えずにClock Track機能を安全に導入することができます。各フェーズでの検証を徹底することで、品質を確保しながら効率的に開発を進めることが可能です。

また、将来の拡張性も考慮した設計により、ユーザーのニーズに応じて機能を拡張していくことができます。