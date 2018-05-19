using SilverNBTLibrary.World;
using System;
using System.Linq;

namespace TUSBBuildBattle
{
    class Program
    {
        static void Main(string[] args)
        {
            // 旧ワールド読み込み
            Console.WriteLine("古いワールドのフォルダをドラッグ&ドロップしてください");
            var oldPath = Console.ReadLine();
            if (!System.IO.Directory.Exists(oldPath))
            {
                Console.WriteLine("ワールドが存在しないため終了します");
            }

            // 新ワールド読み込み
            Console.WriteLine("新しいワールドのフォルダをドラッグ&ドロップしてください");
            var newPath = Console.ReadLine();
            if (!System.IO.Directory.Exists(newPath))
            {
                Console.WriteLine("ワールドが存在しないため終了します");
            }

            // 古いワールド読み込み
            using (var oldworld = World.FromDirectory(oldPath))
            {
                // 新しいワールド読み込み
                using (var newworld = World.FromDirectory(newPath))
                {
                    // 古いワールドの全ディメンション列挙
                    using (var olddim = oldworld.OverWorld)
                    {
                        // 古いワールドのディメンションに対応する新しいワールドのディメンション取得
                        using (var newdim = newworld.GetDimension(olddim.DimensionID))
                        {
                            int i = 0;

                            // 全チャンク取得
                            var oldChunks = olddim.GetAndLoadAllChunks(true);

                            // チャンク数取得
                            var count = oldChunks.Count();

                            // 全チャンク列挙
                            foreach (Chunk andLoadAllChunk in oldChunks)
                            {
                                try
                                {
                                    var cc = andLoadAllChunk.ChunkCoord;
                                    
                                    // 新しいワールドの同じ座標のチャンク
                                    var newChunk = newdim.GetChunkFromChunkCoords(cc);

                                    for (int cx = 0; cx < andLoadAllChunk.Width; cx++)
                                    {
                                        for (int cz = 0; cz < andLoadAllChunk.Length; cz++)
                                        {
                                            // チャンク座標からワールドのX座標に変換
                                            var x = (cc.X << 4) + cx;

                                            // チャンク座標からワールドのZ座標に変換
                                            var z = (cc.Z << 4) + cz;

                                            for (int y = 0; y < andLoadAllChunk.Height; y++)
                                            {
                                                // ID
                                                var id = andLoadAllChunk.GetBlockId(cx, y, cz);

                                                // メタデータ
                                                var meta = andLoadAllChunk.GetBlockMetadata(cx, y, cz);

                                                // 空気ブロック以外のブロックの場合
                                                if (id != 0)
                                                {
                                                    // ブロック設置
                                                    newChunk.SetBlockId(cx, y, cz, id, meta);
                                                    Console.WriteLine("X:{0} Y:{1} Z:{2} ID:{3} Meta:{4}", x, y, z, id, meta);
                                                }
                                            }
                                        }
                                    }

                                    i++;

                                    Console.WriteLine("チャンクを保存しています {0}/{1}", i, count);
                                    newChunk.Save();

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    i++;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
