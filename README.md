# イマドコ
イマドコは、大切な家族が今どこにいるか、音声で位置情報を教えてくれる LINE Clova スキルです。    
位置情報や伝言の登録は、スマホの LINE bot から行います。

機能説明は[コチラのページ](https://mangaclova.github.io/LINEDoko/)を参照してください

[https://mangaclova.github.io/LINEDoko/](https://mangaclova.github.io/LINEDoko/)

この README では、技術的な話だけを書きます。

## 使用サービス

項目|値
----|----
使用言語|C#
開発環境|Visual Studio 2017
クラウド|Microsoft Azure Functions, Azure Table Storage

↓ 以下詳細

### LINE Clova スキルと bot 開発

* *Skill サーバサイド(実行環境)：<a href="https://azure.microsoft.com/ja-jp/services/functions/" title="Azure Functions" target="_blank">Microsoft
    Azure Functions (v2)</a>
* Skill サーバサイド(タイムスタンプの保存)：<a href="https://docs.microsoft.com/ja-jp/azure/storage/common/storage-introduction"
    title="Azure Storage Account" target="_blank">Microsoft Azure ストレージアカウント</a>
* LINE bot：<a href="https://developers.line.me/ja/services/messaging-api/" title="LINE Messaging API" target="_blank">LINE
    Messaging API</a>
* Skill の対話モデル作成：<a href="https://developers.line.me/ja/services/messaging-api/" title="Clova Extensions Kit"
    target="_blank">Clova Extensions Kit</a>
* Clova Extension Kit を C# で使うための SDK：<a href="https://github.com/kenakamu/clova-cek-sdk-csharp" title="Clova CEK SDK C#"
    target="_blank">Clova CEK SDK C#</a>
* LINE Messaging API を C# で使うための SDK：<a href="https://github.com/pierre3/LineMessagingApi" title="LINE Messaging API for C#"
    target="_blank">LINE Messaging API for C#</a>

### Web サイト

* 写真素材：<a href="https://www.pakutaso.com/" title="フリー写真素材ぱくたそ" target="_blank">フリー写真素材ぱくたそ</a>
* Webページホスト：
    <a href="https://pages.github.com/" title="GitHub Pages" target="_blank">GitHub Pages</a>
* Webページフレームワーク：
    <a href="https://getbootstrap.com/" title="Bootstrap" target="_blank">Bootstrap (v4)</a>
* Twitter とか check box とかの汎用アイコンフォント集：
    <a href="https://fontawesome.com/" title="Font Awesome" target="_blank">Font Awesome</a>

### CI/CD

* 自動ビルドと自動デプロイ：<a href="https://azure.microsoft.com/ja-jp/services/devops/" title="Azure DevOps" target="_blank">Azure DevOps</a>
* チームのタスク管理：<a href="https://github.com/MangaClova/LINEDoko/projects/1" title="GitHub Projects" target="_blank">GitHub
    Projects</a>
