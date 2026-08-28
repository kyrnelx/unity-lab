using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Kyrnel.LinkGame
{
    /// <summary>
    /// 重构后的纯逻辑层单元测试：验证每个 Core 类的职责正确、可脱离场景测试。
    /// </summary>
    public class CoreLogicTests
    {
        [Test]
        public void LinkPathfinder_FindsStraightPath()
        {
            var board = new LinkBoard(3, 3);
            board.SetType(new Vector2Int(0, 0), NodeType.CaesarBrown);
            board.SetType(new Vector2Int(2, 0), NodeType.CaesarBrown);

            var finder = new LinkPathfinder(board);
            List<Vector2Int> path = finder.FindPath(new Vector2Int(0, 0), new Vector2Int(2, 0));

            Assert.NotNull(path);
            Assert.AreEqual(2, path.Count);
        }

        [Test]
        public void LinkPathfinder_FindsOneCornerPath()
        {
            var board = new LinkBoard(3, 3);
            board.SetType(new Vector2Int(0, 0), NodeType.CaesarBrown);
            board.SetType(new Vector2Int(1, 2), NodeType.CaesarBrown);

            var finder = new LinkPathfinder(board);
            List<Vector2Int> path = finder.FindPath(new Vector2Int(0, 0), new Vector2Int(1, 2));

            Assert.NotNull(path);
            Assert.AreEqual(3, path.Count);
        }

        [Test]
        public void LinkPathfinder_FindsTwoCornerPath()
        {
            var board = new LinkBoard(3, 3);
            board.SetType(new Vector2Int(0, 1), NodeType.DragonHeadRed);
            board.SetType(new Vector2Int(1, 0), NodeType.DragonHeadRed);
            board.SetType(new Vector2Int(0, 0), NodeType.CaesarBrown);
            board.SetType(new Vector2Int(2, 2), NodeType.CaesarBrown);

            var finder = new LinkPathfinder(board);
            List<Vector2Int> path = finder.FindPath(new Vector2Int(0, 0), new Vector2Int(2, 2));

            Assert.NotNull(path);
            Assert.GreaterOrEqual(path.Count, 3);
        }

        [Test]
        public void LinkPathfinder_RejectsDifferentTypes()
        {
            var board = new LinkBoard(2, 2);
            board.SetType(new Vector2Int(0, 0), NodeType.CaesarBrown);
            board.SetType(new Vector2Int(1, 0), NodeType.DragonHeadRed);

            var finder = new LinkPathfinder(board);
            Assert.Null(finder.FindPath(new Vector2Int(0, 0), new Vector2Int(1, 0)));
        }

        [Test]
        public void LinkPathfinder_RejectsSameNode()
        {
            var board = new LinkBoard(2, 2);
            board.SetType(new Vector2Int(0, 0), NodeType.CaesarBrown);

            var finder = new LinkPathfinder(board);
            Assert.Null(finder.FindPath(new Vector2Int(0, 0), new Vector2Int(0, 0)));
        }

        [Test]
        public void LinkBoard_LoadTypes_UpdatesRemainingCount()
        {
            var board = new LinkBoard(2, 2);
            var types = new List<NodeType>
            {
                NodeType.CaesarBrown, NodeType.CaesarBrown,
                NodeType.DragonHeadRed, NodeType.DragonHeadRed,
            };

            board.LoadTypes(types);

            Assert.AreEqual(4, board.RemainingCount);
            Assert.AreEqual(NodeType.CaesarBrown, board.GetTypeAt(new Vector2Int(0, 0)));
            Assert.AreEqual(NodeType.DragonHeadRed, board.GetTypeAt(new Vector2Int(1, 1)));
        }

        [Test]
        public void LinkBoard_ClearAt_DecrementsRemaining()
        {
            var board = new LinkBoard(2, 2);
            var types = new List<NodeType>
            {
                NodeType.CaesarBrown, NodeType.CaesarBrown,
                NodeType.DragonHeadRed, NodeType.DragonHeadRed,
            };
            board.LoadTypes(types);

            board.ClearAt(new Vector2Int(0, 0));

            Assert.AreEqual(3, board.RemainingCount);
            Assert.IsTrue(board.IsEmptyAt(new Vector2Int(0, 0)));
        }

        [Test]
        public void BoardSolver_DetectsLinkablePair()
        {
            var board = new LinkBoard(2, 1);
            board.SetType(new Vector2Int(0, 0), NodeType.CaesarBrown);
            board.SetType(new Vector2Int(1, 0), NodeType.CaesarBrown);

            var solver = new BoardSolver(board, new LinkPathfinder(board));

            Assert.IsTrue(solver.HasAnyLinkablePair());
            Assert.IsTrue(solver.TryFindLinkablePair(out _, out _));
        }

        [Test]
        public void BoardSolver_ShuffleRemaining_PreservesTypeMultiset()
        {
            var board = new LinkBoard(2, 1);
            board.SetType(new Vector2Int(0, 0), NodeType.CaesarBrown);
            board.SetType(new Vector2Int(1, 0), NodeType.DragonHeadRed);

            var original = new List<NodeType> { board.GetTypeAt(new Vector2Int(0, 0)), board.GetTypeAt(new Vector2Int(1, 0)) };

            var solver = new BoardSolver(board, new LinkPathfinder(board));
            solver.ShuffleRemaining();

            var shuffled = new List<NodeType> { board.GetTypeAt(new Vector2Int(0, 0)), board.GetTypeAt(new Vector2Int(1, 0)) };
            CollectionAssert.AreEquivalent(original, shuffled);
        }

        [Test]
        public void ScoreCalculator_BaseScore()
        {
            var calc = new ScoreCalculator();
            // 0 拐点：基础分 10 + 拐点奖励（3 × 5）= 25
            Assert.AreEqual(ScoreConfig.BaseScore + ScoreConfig.MaxTurnBonusLevel * ScoreConfig.TurnBonusPerLevel, calc.AddScore(0));
        }

        [Test]
        public void ScoreCalculator_ComboBonus_IncreasesWithChain()
        {
            var calc = new ScoreCalculator();
            int first = calc.AddScore(0);
            int second = calc.AddScore(0);
            int third = calc.AddScore(0);

            int basePoints = ScoreConfig.BaseScore + ScoreConfig.MaxTurnBonusLevel * ScoreConfig.TurnBonusPerLevel;
            Assert.AreEqual(basePoints, first);
            Assert.AreEqual(basePoints + ScoreConfig.ComboBonusPerLevel, second);
            Assert.AreEqual(basePoints + ScoreConfig.ComboBonusPerLevel * 2, third);
        }

        [Test]
        public void ScoreCalculator_ResetCombo_ClearsChain()
        {
            var calc = new ScoreCalculator();
            calc.AddScore(0);
            calc.AddScore(0);
            calc.ResetCombo();
            int afterReset = calc.AddScore(0);

            int basePoints = ScoreConfig.BaseScore + ScoreConfig.MaxTurnBonusLevel * ScoreConfig.TurnBonusPerLevel;
            Assert.AreEqual(basePoints, afterReset);
            Assert.AreEqual(1, calc.Combo);
        }

        [Test]
        public void ScoreCalculator_TurnBonus_MoreTurnsLessPoints()
        {
            var calc = new ScoreCalculator();
            int zeroTurns = calc.AddScore(0);
            calc.ResetCombo();
            int threeTurns = calc.AddScore(3);
            calc.ResetCombo();
            int manyTurns = calc.AddScore(10);

            Assert.Greater(zeroTurns, threeTurns);
            Assert.AreEqual(threeTurns, manyTurns);
        }

        private class MemoryStore : IProgressStore
        {
            private readonly Dictionary<string, int> _values = new Dictionary<string, int>();
            public int GetInt(string key, int defaultValue) => _values.TryGetValue(key, out int v) ? v : defaultValue;
            public void SetInt(string key, int value) => _values[key] = value;
            public void Save() { }
        }

        [Test]
        public void LevelProgress_DefaultLevelIsOne()
        {
            var level = new LevelProgress(new MemoryStore());
            level.Load();
            Assert.AreEqual(1, level.CurrentLevel);
        }

        [Test]
        public void LevelProgress_Advance_SavesAndWrapsAtMax()
        {
            var store = new MemoryStore();
            var level = new LevelProgress(store);
            level.Load(); // 默认第 1 关
            level.Advance(); // -> 2
            Assert.AreEqual(2, level.CurrentLevel);

            // 再推进 MaxLevel - 1 次，累计到 999 -> 下一次回绕到 1
            for (int i = 0; i < LevelProgress.MaxLevel - 2; i++)
            {
                level.Advance();
            }

            Assert.AreEqual(LevelProgress.MaxLevel, level.CurrentLevel);

            level.Advance(); // 回绕
            Assert.AreEqual(1, level.CurrentLevel);
            Assert.AreEqual(1, store.GetInt(LevelProgress.LevelPrefsKey, -1));
        }

        [Test]
        public void LevelProgress_SettleScore_AccumulatesAndPersists()
        {
            var store = new MemoryStore();
            var level = new LevelProgress(store);
            level.Load();

            Assert.AreEqual(120, level.SettleLevelScore(120));
            Assert.AreEqual(120, level.TotalScore);
            Assert.AreEqual(150, level.SettleLevelScore(30));
            Assert.AreEqual(150, store.GetInt(LevelProgress.TotalScorePrefsKey, -1));
        }

        [Test]
        public void LevelProgress_TypeCount_GrowsWithLevel_ClampedTo12()
        {
            var level = new LevelProgress(new MemoryStore());
            level.Load();
            Assert.AreEqual(4, level.GetTypeCountForLevel());

            for (int i = 0; i < 20; i++)
            {
                level.Advance();
            }

            Assert.AreEqual(12, level.GetTypeCountForLevel());
        }

        [Test]
        public void BoardLayout_ComputesCellPositions()
        {
            var layout = new BoardLayout(100f, 10f, 5f, 7f);

            Vector2 bottomLeft = layout.GetCellBottomLeft(1, 2);
            Assert.AreEqual(5f + 110f, bottomLeft.x, 0.001f);
            Assert.AreEqual(7f + 220f, bottomLeft.y, 0.001f);

            // pivot=0.5 时格子中心即 anchoredPosition（GetCellCenter == GetCellBottomLeft）
            Vector2 center = layout.GetCellCenter(0, 0);
            Assert.AreEqual(5f, center.x, 0.001f);
            Assert.AreEqual(7f, center.y, 0.001f);
        }

        [Test]
        public void SpawnOrderGenerator_RowByRow_OrderIsCorrect()
        {
            var gen = new SpawnOrderGenerator(2, 3);
            List<Vector2Int> order = gen.Generate(SpawnOrderGenerator.SpawnPattern.RowByRow);

            Assert.AreEqual(6, order.Count);
            Assert.AreEqual(new Vector2Int(0, 0), order[0]);
            Assert.AreEqual(new Vector2Int(0, 1), order[1]);
            Assert.AreEqual(new Vector2Int(1, 0), order[3]);
            Assert.AreEqual(new Vector2Int(1, 2), order[5]);
        }

        [Test]
        public void SpawnOrderGenerator_ContainsAllCells()
        {
            var gen = new SpawnOrderGenerator(3, 3);
            List<Vector2Int> order = gen.Generate(SpawnOrderGenerator.SpawnPattern.OutsideIn);

            Assert.AreEqual(9, order.Count);
            var set = new HashSet<Vector2Int>(order);
            Assert.AreEqual(9, set.Count);
            Assert.IsTrue(set.Contains(new Vector2Int(0, 0)));
            Assert.IsTrue(set.Contains(new Vector2Int(2, 2)));
        }

        // ---------- BoardSizeConfig ----------

        [Test]
        public void BoardSizeConfig_FirstLevelIsSmallBoard()
        {
            var config = new BoardSizeConfig();
            Assert.AreEqual(new Vector2Int(5, 6), config.GetBoardSize(1));
        }

        [Test]
        public void BoardSizeConfig_GrowsEveryTwoLevels()
        {
            var config = new BoardSizeConfig();

            Assert.AreEqual(new Vector2Int(5, 6), config.GetBoardSize(1));
            Assert.AreEqual(new Vector2Int(5, 6), config.GetBoardSize(2));
            Assert.AreEqual(new Vector2Int(6, 7), config.GetBoardSize(3));
            Assert.AreEqual(new Vector2Int(6, 7), config.GetBoardSize(4));
            Assert.AreEqual(new Vector2Int(7, 8), config.GetBoardSize(5));
        }

        [Test]
        public void BoardSizeConfig_AlwaysEvenTotal()
        {
            var config = new BoardSizeConfig();

            // 所有关卡的总格子数必须为偶数（保证可完全配对消除）
            for (int level = 1; level <= 40; level++)
            {
                Vector2Int size = config.GetBoardSize(level);
                Assert.AreEqual(0, size.x * size.y % 2, $"level {level} 棋盘 {size.x}x{size.y} 必须为偶数");
            }
        }

        [Test]
        public void BoardSizeConfig_ClampsAtMaxSize()
        {
            var config = new BoardSizeConfig();
            Vector2Int max = config.GetBoardSize(999);

            Assert.AreEqual(BoardSizeConfig.MaxWidth, max.x);
            Assert.AreEqual(BoardSizeConfig.MaxHeight, max.y);
            Assert.AreEqual(new Vector2Int(8, 12), max);
        }

        [Test]
        public void BoardLayout_Initialized_BoardIsCentered()
        {
            // 初始化 5×6 后，格子（节点中心）应围绕容器锚点 (0,0) 对称
            var layout = new BoardLayout(100f, 17f, 0f, 0f);
            layout.Initialize(5, 6);

            // 最左格中心与最右格中心关于 (0,0) 对称，且中位格中心落在 0
            Assert.AreEqual(0f, (layout.GetCellBottomLeft(0, 0).x + layout.GetCellBottomLeft(4, 5).x) * 0.5f, 0.001f);
            Assert.AreEqual(0f, (layout.GetCellBottomLeft(0, 0).y + layout.GetCellBottomLeft(4, 5).y) * 0.5f, 0.001f);
            Assert.AreEqual(0f, layout.GetCellBottomLeft(0, 0).x + layout.Step * 2f, 0.001f);
            Assert.AreEqual(0f, layout.GetCellBottomLeft(0, 0).y + layout.Step * 2.5f, 0.001f);
        }

        [Test]
        public void BoardLayout_Initialized_DifferentSizesStayCentered()
        {
            var layout = new BoardLayout(100f, 17f, 0f, 0f);

            layout.Initialize(5, 6);
            // 中位格中心应落在 0（格线对称）
            Assert.AreEqual(0f, layout.GetCellBottomLeft(0, 0).x + layout.Step * 2f, 0.001f);
            Assert.AreEqual(0f, layout.GetCellBottomLeft(0, 0).y + layout.Step * 2.5f, 0.001f);

            layout.Initialize(8, 12);
            Assert.AreEqual(0f, layout.GetCellBottomLeft(0, 0).x + layout.Step * 3.5f, 0.001f);
            Assert.AreEqual(0f, layout.GetCellBottomLeft(0, 0).y + layout.Step * 5.5f, 0.001f);
        }
    }
}