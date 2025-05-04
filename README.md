# ReinaS' lilToon NDMF Utility

## これはなに？

lilToon 固有の問題を他の NDMF系非破壊ツールと協調動作させるため、他 lilToon 専用の非破壊 Utility

今は私 Reina_Sakiria が開発していますが、私以外の人がこのツールの開発を引き継いでくれることを願っています。(つまり、 `ReinaS'` の部分がこのツールの名前からなくなること)

## VPM

[Add vpm repo](vcc://vpm/addRepo?url=https://vpm.rs64.net/vpm.json) (TTT と共通)

## 依存パッケージ

- nadena.dev.ndmf `^1.6.0`
- jp.lilxyzw.liltoon `1.8.5`

## TexTransTool との互換性

TexTransTool との併用はサポートされていますが、既知の互換性問題が存在します。

### TTT の [内部レンダーテクスチャーフォーマット](https://ttt.rs64.net/docs/Reference/General/InternalTextureFormat)

`Tools/TexTransTool/Menu` から変更可能な、 [内部レンダーテクスチャーフォーマット](https://ttt.rs64.net/docs/Reference/General/InternalTextureFormat) を `Byte : 8bit - 符号なし整数` 以外にすると、TexTransTool の中間保存時の Texture2D が linear なフォーマットになり、このツールは現状それを正しく扱えないため最終結果が白くなります。

**TexTransTool のコンポーネントと併用する場合は `Byte : 8bit - 符号なし整数` に戻してください！**

## コンポーネントの基本仕様

`ReinaS' lilToon NDMF Utility` 略して `LNU` と表記します。

- Component の左上の チェックボックス の無効化でビルド時に動作しない状態にできます。
- アバターのいかなる GameObject にも付与してよい (EditorOnly な GameObject に付与した場合動作しなくなることには注意)
- コンポーネント間の実行中所は SourceCode `Editor/LNUPlugin.cs` を参考にしてください。(少なくとも TexTransTool のような挙動ではありません。)

## `*** Range` について

これは、内部名称 `MaterialRange` でそのコンポーネントがどのマテリアルたちに影響を及ぼすかを持ちます。

影響範囲の選択には3つ存在し、状況に応じて使い分けてください。

__現在、マテリアル置き換えアニメーションには対応は未実装です！__

### All

アバター内に存在するマテリアルすべてです。

全てに反映したい場合に使用しましょう。

### ParentObject

指定した GameObject たちとその配下にある GameObject に付与された Renderer すべてが持つマテリアルを対象とします。

たとえば、衣装の Root を指定すれば衣装が持つマテリアルがすべて対象になります。

### Manual

手動で Material を指定します。入れたものだけが対象になります。

## コンポーネント一覧

### LNU lilToonMaterialPropertyUnificator

[LI MaterialModifier](https://lilxyzw.github.io/lilycalInventory/ja/docs/components/materialmodifier.html) と同一のものです。

`Unification Target Properties` に目的の `ProperlyName` をいれることで、 `Unification Reference Material` の値で統一します。

### LNU lilToonMaterialOptimizer

[LI MaterialOptimizer](https://lilxyzw.github.io/lilycalInventory/ja/docs/components/materialoptimizer.html) とほぼ同一のものです。

違う点は、環境に [Avatar Optimizer](https://vpm.anatawa12.com/avatar-optimizer/ja/) が存在するときに、 lilToon 固有の最適化のみを行うようになります。

### LNU lilToonMaterialNormalizer

[TTT AtlasTexture](https://ttt.rs64.net/docs/Reference/AtlasTexture) が v0.9.x まで持っていた `PropertyBake`(日本語名:プロパティベイク設定) の機能を持つ存在です。

具体的には、テクスチャに焼き込み可能なプロパティを焼き込みを行い、デフォルト値に値をリセットしたり、 `_Use***` の Property を調節するなど、[TTT AtlasTexture](https://ttt.rs64.net/docs/Reference/AtlasTexture) のマテリアルを結合するときに差異が出づらいように調整する存在です。

実際に併用するときには TTT の実行タイミングには気をつける必要があり、 [TTT AtlasTexture](https://ttt.rs64.net/docs/Reference/AtlasTexture) が `OptimizingPhase` で実行されるようにする必要があり、(TTT AtlasTexture はなにもしなかった場合 `OptimizingPhase` で実行されますが、) `PhaseDefinition` で実行順を制御するときには気をつけてください。
