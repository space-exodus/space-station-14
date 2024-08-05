using Content.Server.Chat.Systems;
using Content.Shared.Coordinates;
using Content.Shared.Exodus.TeleportationZone;
using Robust.Server.GameObjects;
using System.Linq;
using System;
using Content.Shared.Maps;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Content.Server.Decals;

namespace Content.Server.Exodus.TeleportationZone
{
    public sealed class AstroGenMarkerSystem : EntitySystem
    {
        [Dependency] private readonly ITileDefinitionManager _tileDefinitionManager = default!;
        [Dependency] private readonly SharedMapSystem _mapSystem = default!;
        [Dependency] private readonly IEntityManager _entManager = default!;
        [Dependency] private readonly DecalSystem _decals = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<AstroGenMarkerComponent, MapInitEvent>(OnMapInit);
        }

        public static Random _random = new Random();

        private void OnMapInit(EntityUid uid, AstroGenMarkerComponent component, MapInitEvent args)
        {
            double[,] field = GetField();
            var x_form = Transform(uid);
            var Coords = x_form.Coordinates;
            var start_x = Coords.X - 0.5f - 50;
            var start_y = Coords.Y - 0.5f + 51;
            var arrival_pos = x_form.Coordinates;
            var obj_pos = x_form.Coordinates;
            EntityUid start_station_uid = Transform(uid).GridUid!.Value;
            if (!TryComp<MapGridComponent>(start_station_uid, out var start_gridComp))
                return;

            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j] == 0)
                    {
                        var tile = new Vector2i((int)(start_x + i), (int)(start_y - j));
                        var new_pos = _mapSystem.GridTileToLocal(start_station_uid, start_gridComp, tile);
                        var plating = _tileDefinitionManager["Space"];
                        start_gridComp.SetTile(new_pos, new Tile(plating.TileId));
                    }
                    else if (field[i, j] == 3)
                    {
                        var tile = new Vector2i((int)(start_x + i), (int)(start_y - j));
                        var new_pos = _mapSystem.GridTileToLocal(start_station_uid, start_gridComp, tile);
                        var plating = _tileDefinitionManager["Plating"];
                        start_gridComp.SetTile(new_pos, new Tile(plating.TileId));
                        var spawnEnt = _entManager.SpawnEntity("WallPlastitaniumIndestructible", new_pos);
                    }
                    else
                    {
                        var tile = new Vector2i((int)(start_x + i), (int)(start_y - j));
                        var new_pos = _mapSystem.GridTileToLocal(start_station_uid, start_gridComp, tile);
                        var plating = _tileDefinitionManager["FloorDesert"];
                        start_gridComp.SetTile(new_pos, new Tile(plating.TileId));
                        if (field[i, j] == 1)
                        {
                            Random random = new Random();
                            if (random.Next(10) < 1) // этот костыль я уберу. Честное слово)
                            {
                                if (random.Next(10) < 1)
                                {
                                    _decals.TryAddDecal("Busha1", new_pos, out _);
                                }
                                else if (random.Next(10) < 1)
                                {
                                    _decals.TryAddDecal("Busha2", new_pos, out _);
                                }
                                else if (random.Next(10) < 1)
                                {
                                    _decals.TryAddDecal("Bushc1", new_pos, out _);
                                }
                                else
                                {
                                    _decals.TryAddDecal("Bushd4", new_pos, out _);
                                }
                            }
                        }
                        else if (field[i, j] == 2)
                        {
                            var spawnEnt = _entManager.SpawnEntity("WallRock", new_pos);
                        }
                        else if (field[i, j] == -1)
                        {
                            arrival_pos = new_pos;
                        }
                        else if (field[i, j] == -2)
                        {
                            obj_pos = new_pos;
                        }
                    }
                }
            }
            var spawnArrival = _entManager.SpawnEntity("ExodusRoomSpawner_13x13_Hexes_desert_arrival", arrival_pos);
            var spawnObjective = _entManager.SpawnEntity("ExodusRoomSpawner_7x7_Hexes_desert_objective", obj_pos);
        }

        // Lasciate ogne speranza, voi ch'entrate
        public static double[,] GetField()
        {
            int width = 201;
            int height = 201;
            int radius = 50;
            int count_rocks = 50;

            double[,] field = Generate_field(width, height);
            field = Place_main_rock(field, width, height, radius);
            field = Place_rocks(field, count_rocks);
            int[] pos_start = Random_pose(15, 65, 86, 166);
            int[] pos_end = Random_pose(86, 15, 176, 36);
            field = Place_main_points(field, pos_start, pos_end, 11);
            field = Place_pathway(field, pos_start, pos_end, 4);
            field = Restore_main_path(field, pos_start, pos_end, 2);
            field = Outline(field);
            double[,] caves_field = Generate_cave_mask(width, height);
            field = Apply_cave_mask(field, caves_field);
            field = Place_guaranteed_paths(field, pos_start, pos_end);
            field = Place_main_air(field, pos_start, pos_end, 4);

            return field;
        }

        private static double[,] Generate_field(int width, int height)
        {
            double[,] list = new double[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    list[y, x] = 0;
                }
            }
            return list;
        }

        private static double[,] Place_main_rock(double[,] field, int width, int height, int radius)
        {
            double[,] list = field;
            for (int i = 0; i < list.GetLength(0); i++)
            {
                for (int j = 0; j < list.GetLength(1); j++)
                {
                    if (Hypot(i - width / 2, j - height / 2) < radius)
                        list[i, j] = 2;
                }
            }
            return list;
        }

        private static double[,] Place_rocks(double[,] field, int count)
        {
            double[,] rocks = Generate_rocks(count);
            double[,] list = field;
            for (int i = 0; i < list.GetLength(0); i++)
            {
                for (int j = 0; j < list.GetLength(1); j++)
                {
                    for (int ir = 0; ir < rocks.GetLength(0); ir++)
                    {
                        double x = rocks[ir, 0];
                        double y = rocks[ir, 1];
                        double r = rocks[ir, 2];

                        if (Hypot(i - x, j - y) < r)
                        {
                            list[i, j] = 2;
                            break;
                        }
                    }
                }
            }
            return list;
        }

        private static double[,] Generate_rocks(int count)
        {
            int min_rocks = 51;
            int max_rocks = 142;
            int min_radius_rocks = 10;
            int max_radius_rocks = 50;
            Random random = new Random();

            double[,] rocks = new double[count, 3];
            for (int i = 0; i < count; i++)
            {
                rocks[i, 0] = min_rocks + _random.NextDouble() * (max_rocks - min_rocks);
                rocks[i, 1] = min_rocks + _random.NextDouble() * (max_rocks - min_rocks);
                rocks[i, 2] = min_radius_rocks + _random.NextDouble() * (max_radius_rocks - min_radius_rocks);
            }

            return rocks;
        }

        private static int[] Random_pose(int left, int top, int right, int bottom)
        {
            int[] list_pos = new int[2];
            Random random = new Random();
            list_pos[0] = (int)(left + _random.NextDouble() * (right - left + 1));
            list_pos[1] = (int)(top + _random.NextDouble() * (bottom - top + 1));
            return list_pos;
        }

        private static double[,] Place_main_points(double[,] field, int[] pos_start, int[] pos_end, int radius)
        {
            double[,] list = field;
            int[,] points = new int[2, 2];
            points[0, 0] = pos_start[0];
            points[0, 1] = pos_start[1];
            points[1, 0] = pos_end[0];
            points[1, 1] = pos_end[1];

            for (int i = 0; i < list.GetLength(0); i++)
            {
                for (int j = 0; j < list.GetLength(1); j++)
                {
                    for (int ij = 0; ij < points.GetLength(0); ij++)
                    {
                        int x = points[ij, 0];
                        int y = points[ij, 1];

                        if (Hypot(i - y, j - x) < radius)
                        {
                            list[i, j] = 2;
                            break;
                        }
                    }
                }
            }
            return list;
        }

        private static double[,] Place_pathway(double[,] field, int[] pos_start, int[] pos_end, int width)
        {
            double[,] list = field;
            list = Make_pathway(list, pos_start, pos_end);
            list = Raise_pathway(list, width);
            list = Fill_pathway(list);
            return list;
        }

        private static double[,] Make_pathway(double[,] field, int[] pos_start, int[] pos_end)
        {
            double[,] list = field;
            int y_st = Math.Min(pos_start[1], pos_end[1]);
            int y_en = Math.Max(pos_start[1], pos_end[1]);
            int x_st = Math.Min(pos_start[0], pos_end[0]);
            int x_en = Math.Max(pos_start[0], pos_end[0]);

            if (pos_end[0] - pos_start[0] != 0)
            {
                double k = (double)(pos_end[1] - pos_start[1]) / (double)(pos_end[0] - pos_start[0]);
                double a = pos_start[1] - k * pos_start[0];

                for (int y = y_st; y < y_en + 1; y++)
                {
                    int x = (int)((y - a) / k);
                    list[y, x] = 4;
                }

                for (int x = x_st; x < x_en + 1; x++)
                {
                    int y = (int)(x * k + a);
                    list[y, x] = 4;
                }
            }
            else
            {
                for (int y = y_st; y < y_en + 1; y++)
                {
                    list[y, x_st] = 4;
                }
            }
            return list;
        }

        private static double[,] Raise_pathway(double[,] field, int steps)
        {
            double[,] nrfield = (double[,])field.Clone();
            for (int n = 0; n < steps; n++)
            {
                double[,] nfield = (double[,])nrfield.Clone();
                for (int i = 0; i < nfield.GetLength(0); i++)
                {
                    for (int j = 0; j < nfield.GetLength(1); j++)
                    {
                        List<int> h = new List<int>();
                        for (int x = (i - 1); x < (i + 2); x++)
                        {
                            for (int y = (j - 1); y < (j + 2); y++)
                            {
                                if ((x >= 0 & x < nfield.GetLength(0)) & (y >= 0 & y < nfield.GetLength(1)))
                                {
                                    h.Add((int)nrfield[x, y]);
                                }
                            }
                        }
                        if (h.Contains(4))
                            nfield[i, j] = 4;
                    }
                }
                nrfield = nfield;
            }
            return nrfield;
        }

        private static double[,] Fill_pathway(double[,] field)
        {
            double[,] nfield = (double[,])field.Clone();
            for (int i = 0; i < nfield.GetLength(0); i++)
            {
                for (int j = 0; j < nfield.GetLength(1); j++)
                {
                    if (nfield[i, j] == 4)
                        nfield[i, j] = 2;
                }
            }
            return nfield;
        }

        private static double[,] Restore_main_path(double[,] field, int[] pos_start, int[] pos_end, int width)
        {
            double[,] nfield = (double[,])field.Clone();
            nfield = Place_main_points(nfield, pos_start, pos_end, width);
            nfield = Place_pathway(nfield, pos_start, pos_end, width);
            nfield = Place_entrance(nfield, pos_start, pos_end);
            return nfield;
        }

        private static double[,] Place_entrance(double[,] field, int[] pos_start, int[] pos_end)
        {
            double[,] nfield = (double[,])field.Clone();
            nfield[pos_start[1], pos_start[0]] = -1;
            nfield[pos_end[1], pos_end[0]] = -2;
            return nfield;
        }

        private static double[,] Outline(double[,] field)
        {
            double[,] nfield = (double[,])field.Clone();
            for (int i = 0; i < nfield.GetLength(0); i++)
            {
                for (int j = 0; j < nfield.GetLength(1); j++)
                {
                    if (nfield[i, j] == 2)
                    {
                        List<int> h = new List<int>();
                        for (int x = (i - 1); x < (i + 2); x++)
                        {
                            for (int y = (j - 1); y < (j + 2); y++)
                            {
                                if ((x >= 0 & x < nfield.GetLength(0)) & (y >= 0 & y < nfield.GetLength(1)))
                                    h.Add((int)nfield[x, y]);
                            }
                        }

                        if (h.Contains(0) || 0 == i || i == (nfield.GetLength(0) - 1) || 0 == j || j == (nfield.GetLength(1) - 1))
                            nfield[i, j] = 3;
                    }
                }
            }
            return nfield;
        }

        private static double[,] Generate_cave_mask(int width, int height)
        {
            double caves_steps = Math.Ceiling(Math.Max(Math.Log(width, 2), Math.Log(height, 2)));
            int caves_smooths = 2;
            double[,] caves_field = Offset(White_noise(2, 2), -0.5);
            for (int step = 2; step < caves_steps + 1; step++)
            {
                caves_field = Overlay(Resize(Scale(caves_field, 0.6), (int)Math.Pow(2, step), (int)Math.Pow(2, step)),
                                      Scale(Offset(White_noise((int)Math.Pow(2, step), (int)Math.Pow(2, step)), -0.5), 1));
            }
            caves_field = Normalize(caves_field);
            for (int i = 0; i < caves_smooths; i++)
            {
                caves_field = Noise_smooth(caves_field);
            }
            caves_field = Get_cave_mask(caves_field);
            caves_field = Crop(caves_field, width, height);
            return caves_field;
        }

        private static double[,] Offset(double[,] field, double value)
        {
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    field[i, j] = field[i, j] + value;
                }
            }
            return field;
        }

        private static double[,] White_noise(int width, int height)
        {
            Random random = new Random();
            double[,] field = new double[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    field[i, j] = _random.NextDouble();
                }
            }
            return field;
        }

        private static double[,] Overlay(double[,] field1, double[,] field2)
        {
            double[,] field = new double[field1.GetLength(0), field1.GetLength(1)];
            for (int i = 0; i < field1.GetLength(0); i++)
            {
                for (int j = 0; j < field1.GetLength(1); j++)
                {
                    field[i, j] = field1[i, j] + field2[i, j];
                }
            }
            return field;
        }

        private static double[,] Resize(double[,] field, int width, int height)
        {
            double[,] nfield = new double[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    nfield[i, j] = field[i * field.GetLength(1) / height, j * field.GetLength(0) / width];
                }
            }
            return nfield;
        }

        private static double[,] Scale(double[,] field, double value)
        {
            double[,] nfield = new double[field.GetLength(0), field.GetLength(1)];
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    nfield[i, j] = field[i, j] * value;
                }
            }
            return nfield;
        }

        private static double[,] Normalize(double[,] field)
        {
            double[,] nfield = new double[field.GetLength(0), field.GetLength(1)];
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    nfield[i, j] = Math.Min(1, Math.Max(0, field[i, j]));
                }
            }
            return nfield;
        }

        private static double[,] Noise_smooth(double[,] field)
        {
            double[,] nfield = (double[,])field.Clone();
            for (int i = 0; i < nfield.GetLength(0); i++)
            {
                for (int j = 0; j < nfield.GetLength(1); j++)
                {
                    nfield[i, j] = Get_aver(field, j, i);
                }
            }
            return nfield;
        }

        private static double Get_aver(double[,] field, int x, int y)
        {
            List<double> h = new List<double>();
            for (int i = (y - 1); i < (y + 2); i++)
            {
                for (int j = (x - 1); j < (x + 2); j++)
                {
                    if ((i >= 0 & i < field.GetLength(0)) & (j >= 0 & j < field.GetLength(1)))
                        h.Add(field[i, j]);
                }
            }
            return h.Sum() / h.Count();
        }

        private static double[,] Get_cave_mask(double[,] field)
        {
            double[,] nfield = field;
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    double element = field[i, j];
                    if ((0.09 < element & element < 0.91) & (element < 0.20 || element > 0.8))
                    {
                        field[i, j] = 0;
                    }
                    else
                    {
                        nfield[i, j] = 1;
                    }
                }
            }
            return nfield;
        }

        private static double[,] Crop(double[,] field, int width, int height)
        {
            double[,] nfield = new double[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    nfield[i, j] = field[i, j];
                }
            }
            return nfield;
        }

        private static double[,] Apply_cave_mask(double[,] field, double[,] mask)
        {
            double[,] nfield = field;
            for (int i = 0; i < nfield.GetLength(0); i++)
            {
                for (int j = 0; j < nfield.GetLength(1); j++)
                {
                    if (field[i, j] == 2 & mask[i, j] == 0)
                        nfield[i, j] = 1;
                }
            }
            return nfield;
        }

        private static double[,] Place_guaranteed_paths(double[,] field, int[] pos_start, int[] pos_end)
        {
            double[,] nfield = (double[,])field.Clone();
            List<int[]> striders_direction_masks = new List<int[]> { new int[] { 1, 1, 1, 1 }, new int[] { 1, 1, 1, 1 } };
            int[] ps = (int[])pos_start.Clone();
            int[][] striders = Enumerable.Repeat(ps, striders_direction_masks.Count).ToArray();
            while (striders.All(strider_pos => !(strider_pos[0] == pos_end[0] & strider_pos[1] == pos_end[1])))
            {
                for (int i = 0; i < striders.GetLength(0); i++)
                {
                    var obj = Random_step(nfield, striders[i], pos_end, striders_direction_masks[i]);
                    striders[i][0] = obj.Item1;
                    striders[i][1] = obj.Item2;
                    if (striders[i] != pos_end)
                        nfield = Trace(nfield, striders[i]);
                }
            }
            return nfield;
        }

        private static double[,] Trace(double[,] field, int[] pos)
        {
            field[pos[1], pos[0]] = 1;
            return field;
        }

        private static (int, int) Random_step(double[,] field, int[] pos, int[] target, int[] mask)
        {
            int x = pos[0];
            int y = pos[1];
            List<List<object>> h = new List<List<object>>();
            List<(int, int)> moves = new List<(int, int)> { (0, -1), (1, 0), (0, 1), (-1, 0) };
            var path = Negative_overlay(target, pos);
            var length = Hypot(path[0], path[1]);
            for (int i = 0; i < moves.Count(); i++)
            {
                (int, int) m = moves[i];
                double v = 0;

                if (field[y + m.Item2, x + m.Item1] == 3)
                {
                    v = 0;
                }
                else if (field[y + m.Item2, x + m.Item1] == 2)
                {
                    v = 0.1;
                }
                else if (field[y + m.Item2, x + m.Item1] == 1)
                {
                    v = 1;
                }
                else if (field[y + m.Item2, x + m.Item1] == -2)
                {
                    v = 10;
                }
                double k = 0.3 + Math.Max(0, Multiply(m, path).Sum()) / length;
                v *= k;
                v *= mask[i];
                List<object> h1 = new List<object> { m, v };
                h.Add(h1);
            }
            var m1 = Choice_weight(h);
            return (x + m1.Item1, y + m1.Item2);
        }

        private static (int, int) Choice_weight(List<List<object>> h)
        {
            Random random = new Random();
            double[] pr = new double[h.Count + 1];
            pr[0] = 0;
            for (int i = 0; i < h.Count; i++)
            {
                pr[i + 1] = (pr[i] + (double)h[i][1]);
            }
            double r = _random.NextDouble() * pr[^1];
            for (int j = 1; j < pr.Length; j++)
            {
                if (r < pr[j])
                {
                    return ((int, int))h[j - 1][0];
                }
            }
            return ((int, int))h[0][0];
        }

        private static int[] Multiply((int, int) field1, int[] field2)
        {
            int[] nfield = new int[2];
            nfield[0] = field1.Item1 * field2[0];
            nfield[1] = field1.Item2 * field2[1];
            return nfield;
        }

        private static int[] Negative_overlay(int[] field1, int[] field2)
        {
            int[] nfield = new int[2];
            nfield[0] = field1[0] - field2[0];
            nfield[1] = field1[1] - field2[1];
            return nfield;
        }

        private static double[,] Place_main_air(double[,] field, int[] pos_start, int[] pos_end, int radius)
        {
            double[,] nfield = (double[,])field.Clone();
            int[,] points = new int[2, 2];
            points[0, 0] = pos_start[0];
            points[0, 1] = pos_start[1];
            points[1, 0] = pos_end[0];
            points[1, 1] = pos_end[1];

            for (int i = 0; i < nfield.GetLength(0); i++)
            {
                for (int j = 0; j < nfield.GetLength(1); j++)
                {
                    for (int ij = 0; ij < points.GetLength(0); ij++)
                    {
                        int x = points[ij, 0];
                        int y = points[ij, 1];

                        if (Hypot(i - y, j - x) < radius & field[i, j] == 2)
                        {
                            nfield[i, j] = 1;
                            break;
                        }
                    }
                }
            }
            nfield = Place_entrance(nfield, pos_start, pos_end);
            return nfield;
        }

        private static double Hypot(double a, double b)
        {
            return Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
        }
    }
}
