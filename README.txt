PFSnip
(C) Project Flower 2022

ウィンドウのスクリーン ショットを作成します。

概要:
  Windows に搭載されている Snipping Tool はスクリーン ショットの作成に利用できますが、
  プレビューにマウス ポインタが移動するとポインタが非常に見づらくなり、位置がわかりにくくなります。
  PFSnip は、Snipping Tool とだいたい同じ機能を利用できます。

動作環境:
  Microsoft Windows 10
  .NET Framework 4.8

ファイル
  PFSnip.exe
    実行ファイルです。
  PFSinip.exe.config
    設定ファイル
  README.txt
    このファイル

使い方:
  1. [Mode]から[File]又は[Clipboard]を選択して下さい。
     [File]を選択するとスクリーンショットを画像ファイルに保存します。
     [Clipboard]を選択するとスクリーンショットをクリップボードにコピーします。

  2. キャプチャするタイミングを遅延させる場合は、[Delay:]に遅延させる秒数を入力して下さい。

  3. [Capture]をクリックすると、現在のデスクトップ全体をキャプチャします。
     マウス ポインタを動かして、保存したいウィンドウの領域を選択して下さい。
     ウィンドウがある領域にポインタが移動すると、赤い枠で領域を強調します。

  4. 領域が選択された状態でクリックすると、
     ファイルに保存する場合はダイアログが開くので、ファイルを指定して[保存]をクリックして下さい。

     クリップボードにコピーする場合は、クリックするとその領域の画像データががクリップボードにコピーされます。

     領域が選択されない場所をクリックするか、エスケープ キーを押すと元の画面に戻ります。

  5. [One More]をクリックすると、直前に保存又はコピーした画像をもう一度保存もしくはコピーすることができます。
     [Mode]を変更すれば、直前にファイルに保存した画像をクリップボードにコピー、
     もしくは直前にコピーした画像を保存することも可能です。

設定:
  設定ファイル PFSnip.config の内容を変更することで、動作を変更できます。
    ・PreviewFrameColor
      プレビュー時のウィンドウを強調する枠の色 (既定値: Red (赤))
    ・PreviewShadowAlpha
      プレビュー時の影の透明度 (0～255) (既定値: 128 (50％))
    ・PreviewShadowColor
      プレビュー時の影の色 (既定値: Black (黒))

  <configuration>
    <userSettings>
      <PFSnip.Properties.Settings>
        <setting name="PreviewFrameColor" serializeAs="String">
          <value>Red</value>
        </setting>
        <setting name="PreviewShadowAlpha" serializeAs="String">
          <value>128</value>
        </setting>
        <setting name="PreviewShadowColor" serializeAs="String">
          <value>Black</value>
        </setting>
      </PFSnip.Properties.Settings>
    </userSettings>
  </configuration>

制限:
  自由形式、四角形、全画面領域の切り取りはできません。
  他のウィンドウの裏に隠れて見えない領域を選択した場合、
  重なったままの領域がキャプチャされます。

  一部のウィンドウは領域を正しく認識できません(タイトルバーに文字が無いウィンドウや特殊なウィンドウ等)。

  Windows 10 以外での動作確認は行っておりません。
