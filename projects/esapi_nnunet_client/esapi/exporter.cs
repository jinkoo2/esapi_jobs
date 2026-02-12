using itk.simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace esapi
{
    public class exporter
    {

        public static void print(string msg)
        {
            global_variables.print(msg);
        }

        public static void error(string msg)
        {
            global_variables.print(msg);
        }

        public static string[] replace(string[] from, string search, string replace_with)
        {
            System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
            foreach (string line in from)
            {
                list.Add(line.Replace(search, replace_with));
            }

            return list.ToArray();
        }

        public static bool override_pt_info = true;

        public static void write_pt_info(Patient pt, string pt_dir)
        {
            string fn = $"write_pt_info(pt={_Id(pt)}, pt_dir={pt_dir})";
            print(fn);

            string info_file = filesystem.join(pt_dir, "info.txt");

            // skip if file exists
            if (filesystem.file_exists(info_file) && (!override_pt_info))
                return;

            StringBuilder sb = new StringBuilder();
            {
                sb.AppendLine("Id=" + uuid);
                //sb.AppendLine("LastName=" + pt.LastName);
                //sb.AppendLine("FirstName=" + pt.FirstName);
                sb.AppendLine("Sex=" + pt.Sex);
                sb.AppendLine("DateOfBirth=" + pt.DateOfBirth.ToString());

                // get primary oncologist
                string pid = piduuid.uuid2pid(uuid);
                if (pid != null && pid.Trim() != "")
                {
                    string[] list = variandb_query.get_PrimaryOncologist_Of_Pid(pid);
                    if (list.Length == 2) // fist line is the header. Usually, Length should be 2
                    {
                        string line = list[1];
                        helper.print("PrimaryOncologist=" + line);
                        sb.AppendLine("PrimaryOncologist=" + line);
                    }
                    else if (list.Length > 2)
                    {
                        string line = list[1];
                        helper.print("More than one primary oncologist found. Just using the first one: PrimaryOncologist=" + line);
                        sb.AppendLine("PrimaryOncologist=" + line);
                    }
                    else
                    {
                        helper.print("No primary oncologist found.");
                        sb.AppendLine("PrimaryOncologist=");
                    }
                }
            }
            System.IO.File.WriteAllText(info_file, sb.ToString());

            // log
            if (global_variables.make_export_log)
                export_log.new_pt_info_file_created(info_file);
        }

        public static void write_plansetup_info(PlanSetup ps, string dir)
        {
            string fn = $"write_plansetup_info(ps={_Id(ps)}, pt_dir={dir})";
            print(fn);

            string info_file = filesystem.join(dir, "info.txt");

            // skip if file exists
            if (filesystem.file_exists(info_file))
                return;

            StringBuilder sb = new StringBuilder();
            {
                sb.AppendLine("Id=" + ps.Id);
                sb.AppendLine("PlanType=" + ps.PlanType);
                if (ps.StructureSet != null)
                    sb.AppendLine("StructureSet.Id=" + ps.StructureSet.Id);
                if (ps.StructureSet != null && ps.StructureSet.Image != null)
                    sb.AppendLine("Image.Id=" + ps.StructureSet.Image.Id);
                sb.AppendLine("ApprovalStatus=" + ps.ApprovalStatus);
                sb.AppendLine("Comment=" + ps.Comment);
                sb.AppendLine("Course.Id=" + ps.Course.Id);
                sb.AppendLine("CreationDateTime=" + ps.CreationDateTime.ToString());
                sb.AppendLine("CreationUserName=" + ps.CreationUserName);
                sb.AppendLine("CreationDateTime=" + ps.HistoryDateTime.ToString());
                sb.AppendLine("CreationUserName=" + ps.HistoryUserName);
                sb.AppendLine("ApprovalStatus=" + ps.IsDoseValid);
                sb.AppendLine("ApprovalStatus=" + ps.IsTreated);
                sb.AppendLine("SeriesUID=" + ps.SeriesUID);
                sb.AppendLine("Series.Id=" + ps.Series.Id);
                //sb.AppendLine("TotalPrescribedDose=" + ps.TotalPrescribedDose.ToString());
                sb.AppendLine("TotalPrescribedDose=" + ps.TotalDose.Dose.ToString());
                sb.AppendLine("TreatmentApprovalDate=" + ps.TreatmentApprovalDate.ToString());
                sb.AppendLine("TreatmentApprover=" + ps.TreatmentApprover);
                sb.AppendLine("TreatmentOrientation=" + ps.TreatmentOrientation);
                sb.AppendLine("UID=" + ps.UID);
                //if(ps.UniqueFractionation != null)
                //{
                //    append_line(info_file, "NumberOfFractions=" + ps.UniqueFractionation.NumberOfFractions);
                //    append_line(info_file, "PrescribedDosePerFraction=" + ps.UniqueFractionation.PrescribedDosePerFraction);
                //}
                sb.AppendLine("TreatmentOrientation=" + ps.TreatmentOrientation);
            }
            System.IO.File.WriteAllText(info_file, sb.ToString());

            // log
            if (global_variables.make_export_log)
                export_log.new_pt_info_file_created(info_file);
        }

        public static string str(Object o)
        {
            return string.Format("{0}", o);
        }

        public static string mat2d_to_string(float[,] leafPositions)
        {
            int R = leafPositions.GetLength(0);
            int C = leafPositions.GetLength(1);

            List<string> rows = new List<string>();
            for (int r = 0; r < R; r++)
            {
                List<string> cols = new List<string>();
                for (int c = 0; c < C; c++)
                {
                    cols.Add(str(leafPositions[r, c]));
                }
                rows.Add(string.Join(",", cols));
            }
            return string.Join("/", rows);
        }

        public static string VVector_to_string(VVector v)
        {
            return string.Format("{0},{1},{2}", v.x, v.y, v.z);
        }

        public static void write_beam_info(Beam b, string dir)
        {
            string fn = $"write_beam_info(b={_Id(b)}, dir={dir})";
            print(fn);

            string info_file = filesystem.join(dir, "info.txt");

            // skip if file exists
            if (filesystem.file_exists(info_file))
                return;

            StringBuilder sb = new StringBuilder();
            {
                sb.AppendLine("Id=" + b.Id);
                sb.AppendLine("ToleranceTableLabel=" + b.ToleranceTableLabel);
                sb.AppendLine("Technique=" + b.Technique);
                sb.AppendLine("Meterset.Unit=" + b.Meterset.Unit);
                sb.AppendLine("Meterset.Value=" + b.Meterset.Value);

                //if(b.ExternalBeam != null)
                //{
                //    append_line(info_file, "ExternalBeam.Id=" + b.ExternalBeam.Id);
                //    append_line(info_file, "ExternalBeam.MachineModel=" + b.ExternalBeam.MachineModel);
                //}

                sb.AppendLine("EnergyModeDisplayName=" + b.EnergyModeDisplayName);
                sb.AppendLine("DoseRate=" + b.DoseRate);
                if (b.MLC != null)
                {
                    sb.AppendLine("MLC.Id=" + b.MLC.Id);
                    sb.AppendLine("MLC.Model=" + b.MLC.Model);
                }

                sb.AppendLine("DoseRate=" + b.DoseRate);
                sb.AppendLine("SSD=" + str(b.SSD));
                sb.AppendLine("AverageSSD=" + str(b.AverageSSD));
                sb.AppendLine("IsocenterPosition=" + VVector_to_string(b.IsocenterPosition));
                sb.AppendLine("WeightFactor=" + b.WeightFactor);

                sb.AppendLine("ControlPoints.Count=" + b.ControlPoints.Count);
                List<string> jawPositions = new List<string>();
                List<string> colAngles = new List<string>();
                List<string> gantryAngles = new List<string>();
                List<string> leafPositions = new List<string>();
                List<string> metersetWeights = new List<string>();
                List<string> patientSupportAngles = new List<string>();
                List<string> tableTopLatPositions = new List<string>();
                List<string> tableTopLngPositions = new List<string>();
                List<string> tableTopVrtPositions = new List<string>();

                foreach (ControlPoint cp in b.ControlPoints)
                {
                    jawPositions.Add(str(cp.JawPositions));
                    colAngles.Add(str(cp.CollimatorAngle));
                    gantryAngles.Add(str(cp.GantryAngle));
                    leafPositions.Add(mat2d_to_string(cp.LeafPositions));
                    metersetWeights.Add(str(cp.MetersetWeight));
                    patientSupportAngles.Add(str(cp.PatientSupportAngle));
                    tableTopLatPositions.Add(str(cp.TableTopLateralPosition));
                    tableTopLngPositions.Add(str(cp.TableTopLongitudinalPosition));
                    tableTopVrtPositions.Add(str(cp.TableTopVerticalPosition));
                }
                sb.AppendLine("ControlPoints.JawPositionsList=" + string.Join("|", jawPositions));
                sb.AppendLine("ControlPoints.CollimatorAngleList=" + string.Join("|", colAngles));
                sb.AppendLine("ControlPoints.GantryAngleList=" + string.Join("|", gantryAngles));
                sb.AppendLine("ControlPoints.LeafPositionsList=" + string.Join("|", leafPositions));
                sb.AppendLine("ControlPoints.MetersetWeightList=" + string.Join("|", metersetWeights));
                sb.AppendLine("ControlPoints.PatientSupportAngleList=" + string.Join("|", patientSupportAngles));
                sb.AppendLine("ControlPoints.TableTopLateralPositionList=" + string.Join("|", tableTopLatPositions));
                sb.AppendLine("ControlPoints.TableTopLongitudinalPositionList=" + string.Join("|", tableTopLngPositions));
                sb.AppendLine("ControlPoints.TableTopVerticalPositionList=" + string.Join("|", tableTopVrtPositions));

                // Block
                sb.AppendLine("Blocks.Count=" + b.Blocks.ToArray().Length);
                List<string> blkIds = new List<string>();
                foreach (Block blk in b.Blocks)
                {
                    blkIds.Add(str(blk.Id));
                }
                sb.AppendLine("ControlPoints.BlockIdList=" + string.Join("|", blkIds));

                // Bolus
                sb.AppendLine("Boluses.Count=" + b.Boluses.ToArray().Length);
                List<string> bolusIds = new List<string>();
                foreach (Bolus bolus in b.Boluses)
                {
                    bolusIds.Add(str(bolus.Id));
                }
                sb.AppendLine("ControlPoints.BolusIdList=" + string.Join("|", bolusIds));
            }
            System.IO.File.WriteAllText(info_file, sb.ToString());

            // log
            if (global_variables.make_export_log)
                export_log.new_beam_info_file_created(info_file);

        }

        public static void export_beam(Beam b, string dir)
        {
            string fn = $"export_beam(b={_Id(b)}, dir={dir})";
            print(fn);

            write_beam_info(b, dir);
        }

        public static void export_plansetup(PlanSetup ps, string dir)
        {
            string fn = $"export_plansetup(ps={_Id(ps)}, dir={dir})";
            print(fn);


            write_plansetup_info(ps, dir);

            foreach (Beam b in ps.Beams)
            {
                if (b.IsSetupField)
                    continue;

                string beam_dir = filesystem.join(dir, string.Format("beam_{0}", b.BeamNumber), true);
                export_beam(b, beam_dir);
            }
        }

        static string uuid = "";

        public static bool is_irreg_plan(PlanSetup ps)
        {
            // irreg plan has image, and the size is 1x1x1
            if (ps.StructureSet != null && ps.StructureSet.Image != null && ps.StructureSet.Image.XSize == 1)
                return true;
            else
                return false;
        }

        public static void export_plansetup(Patient p, Course c, PlanSetup ps, string data_root)
        {
            string fn = $"export_plansetup(p={_Id(p)},c={_Id(c)},ps={_Id(ps)}, data_root={data_root})";
            print(fn);

            if (ps == null)
            {
                error("export_plansetup() - ps parameter is null.");
                return;
            }

            if (is_irreg_plan(ps))
            {
                print("This is an irreg plan. skipping...");
                return;
            }

            //// plan revisions are not exported.
            //if (ps.Id.Contains(':'))
            //{
            //    error("export_plansetup() - ps.Id has ':'. So, skipping...");
            //    return;
            //}

            // get uuid for the pid
            uuid = "";
            {
                // pid to uuid conversion file
                string id2uuid_file = System.IO.Path.Combine(global_variables.data_root_secure, "id2uuid.txt");
                if (!System.IO.File.Exists(id2uuid_file))
                {
                    throw new Exception("File not found: " + id2uuid_file);
                }

                // note better to use a dictionary, inspead of param, for efficincy. 
                param param = new param(id2uuid_file);

                try
                {
                    uuid = param.get_value(p.Id);
                }
                catch (Exception e)
                {
                    // new uuid (new pt)
                    uuid = System.Guid.NewGuid().ToString();

                    // update the conversion files
                    filesystem.appendline(id2uuid_file, string.Format("{0}={1}", p.Id, uuid));

                    //log
                    if (global_variables.make_export_log)
                        export_log.new_uuid_created(uuid);

                    print("new uuid=" + uuid);
                }
                print(uuid);
            }

            //try
            {
                string pt_dir = filesystem.join(data_root, uuid);
                if (!filesystem.dir_exists(pt_dir))
                {
                    filesystem.mkdir(pt_dir);
                    if (global_variables.make_export_log)
                        export_log.new_pt_dir_created(pt_dir);
                }

                print(string.Format("write_pt_info({0})", p.Id));
                write_pt_info(p, pt_dir);

                string cs_list_dir = filesystem.join(pt_dir, "cs_list", true);
                string cs_dir = filesystem.join(cs_list_dir, filesystem.encode_path(c.Id));
                if (!filesystem.dir_exists(cs_dir))
                {
                    filesystem.mkdir(cs_dir);
                    if (global_variables.make_export_log)
                        export_log.new_cs_dir_created(cs_dir);
                }

                // export image
                // irreg plan has image, and the size is 1x1x1
                if (ps.StructureSet != null && ps.StructureSet.Image != null && ps.StructureSet.Image.XSize > 10)
                {
                    string img_list_dir = filesystem.join(pt_dir, "img_list", true);
                    string img_dir = filesystem.join(img_list_dir, filesystem.encode_path(ps.StructureSet.Image.Id));
                    if (!filesystem.dir_exists(img_dir))
                    {
                        filesystem.mkdir(img_dir);
                        if (global_variables.make_export_log)
                            export_log.new_img_dir_created(img_dir);
                    }

                    export_image(ps.StructureSet.Image, img_dir, "img");
                }

                // export structure set
                if (ps.StructureSet != null)
                {
                    string sset_list_dir = filesystem.join(pt_dir, "sset_list", true);
                    string sset_dir = filesystem.join(sset_list_dir, filesystem.encode_path(ps.StructureSet.Id));

                    if (!filesystem.dir_exists(sset_dir))
                    {
                        filesystem.mkdir(sset_dir);
                        if (global_variables.make_export_log)
                            export_log.new_sset_dir_created(sset_dir);
                    }

                    print(string.Format("export_structureset({0})", ps.StructureSet.Id));
                    export_structureset(ps.StructureSet, sset_dir);
                }

                // export plansetup
                {
                    string ps_list_dir = filesystem.join(pt_dir, "ps_list", true);
                    string ps_dir = filesystem.join(ps_list_dir, filesystem.encode_path(ps.Id), true);
                    print(string.Format("export_plansetup({0})", ps.Id));
                    export_plansetup(ps, ps_dir);
                }


                // write info
                //string info_file = join(img_dir, "exporter.info.txt");
                //{
                //  write_line(info_file, "ImagingOrientation=" + ps.StructureSet.Image.ImagingOrientation.ToString());
                //  append_line(info_file, "TreatmentOrientation=" + ps.TreatmentOrientation.ToString());
                //  append_line(info_file, "PlanType=" + ps.PlanType.ToString());
                //  append_line(info_file, "PlanSetup.Id=" + ps.Id.ToString());
                //}

                //export_structure_as_mask_image(ps.StructureSet.Image, s, img_dir, encode_path(s.Id));
            }
            //catch (Exception e)
            //{
            //    Console.Error.WriteLine("Something went wrong... skipping..." + e.Message);
            //}
        }

        private static string _Id(ApiDataObject obj)
        {
            return (obj != null) ? obj.Id : "null";
        }

        public static void export_plansetup_with_sid_like(Patient p, Course c, PlanSetup ps, string sid_like, string data_root)
        {
            string fn = $"export_plansetup_with_sid_like(p={_Id(p)}, c={_Id(c)}, ps={_Id(ps)}, sid_like={sid_like}, data_root={data_root})";
            helper.print(fn);

            if (ps == null)
            {
                error("ps parameter is null.");
                return;
            }

            //// plan revisions are not exported.
            //if (ps.Id.Contains(':'))
            //{
            //    print("ps.Id has ':'. So, skipping...");
            //    return;
            //}

            // get uuid for the pid
            print("Getting uuid of pid...");
            uuid = "";
            {
                // pid to uuid conversion file
                string id2uuid_file = System.IO.Path.Combine(global_variables.data_root_secure, "id2uuid.txt");
                if (!System.IO.File.Exists(id2uuid_file))
                {
                    throw new Exception("File not found: " + id2uuid_file);
                }

                // note better to use a dictionary, inspead of param, for efficincy. 
                param param = new param(id2uuid_file);

                try
                {
                    uuid = param.get_value(p.Id);
                }
                catch (Exception e)
                {
                    // new uuid (new pt)
                    uuid = System.Guid.NewGuid().ToString();

                    // update the conversion files
                    filesystem.appendline(id2uuid_file, string.Format("{0}={1}", p.Id, uuid));

                    //log
                    if (global_variables.make_export_log)
                        export_log.new_uuid_created(uuid);

                    print("new uuid=" + uuid);
                }
                print(uuid);
            }

            //try
            {
                string pt_dir = filesystem.join(data_root, uuid);
                if (!filesystem.dir_exists(pt_dir))
                {
                    filesystem.mkdir(pt_dir);
                    if (global_variables.make_export_log)
                        export_log.new_pt_dir_created(pt_dir);
                }

                print(string.Format("write_pt_info({0})", p.Id));
                write_pt_info(p, pt_dir);

                string cs_list_dir = filesystem.join(pt_dir, "cs_list", true);
                string cs_dir = filesystem.join(cs_list_dir, filesystem.encode_path(c.Id));
                if (!filesystem.dir_exists(cs_dir))
                {
                    filesystem.mkdir(cs_dir);
                    if (global_variables.make_export_log)
                        export_log.new_cs_dir_created(cs_dir);
                }

                // export image
                if (ps.StructureSet != null && ps.StructureSet.Image != null)
                {
                    string img_list_dir = filesystem.join(pt_dir, "img_list", true);
                    string img_dir = filesystem.join(img_list_dir, filesystem.encode_path(ps.StructureSet.Image.Id));
                    if (!filesystem.dir_exists(img_dir))
                    {
                        filesystem.mkdir(img_dir);
                        if (global_variables.make_export_log)
                            export_log.new_img_dir_created(img_dir);
                    }

                    print(string.Format("export_image({0})", ps.StructureSet.Image.Id));
                    export_image(ps.StructureSet.Image, img_dir, "img");
                }

                // export structure set
                if (ps.StructureSet != null)
                {
                    string sset_list_dir = filesystem.join(pt_dir, "sset_list", true);
                    string sset_dir = filesystem.join(sset_list_dir, filesystem.encode_path(ps.StructureSet.Id));

                    if (!filesystem.dir_exists(sset_dir))
                    {
                        filesystem.mkdir(sset_dir);
                        if (global_variables.make_export_log)
                            export_log.new_sset_dir_created(sset_dir);
                    }

                    print(string.Format("export_structureset({0})", ps.StructureSet.Id));
                    export_structureset_with_sid_like(ps.StructureSet, sid_like, sset_dir);
                }

                // export plansetup
                {
                    string ps_list_dir = filesystem.join(pt_dir, "ps_list", true);
                    string ps_dir = filesystem.join(ps_list_dir, filesystem.encode_path(ps.Id), true);
                    print(string.Format("export_plansetup({0})", ps.Id));
                    export_plansetup(ps, ps_dir);
                }


                // write info
                //string info_file = join(img_dir, "exporter.info.txt");
                //{
                //  write_line(info_file, "ImagingOrientation=" + ps.StructureSet.Image.ImagingOrientation.ToString());
                //  append_line(info_file, "TreatmentOrientation=" + ps.TreatmentOrientation.ToString());
                //  append_line(info_file, "PlanType=" + ps.PlanType.ToString());
                //  append_line(info_file, "PlanSetup.Id=" + ps.Id.ToString());
                //}

                //export_structure_as_mask_image(ps.StructureSet.Image, s, img_dir, encode_path(s.Id));
            }
            //catch (Exception e)
            //{
            //    Console.Error.WriteLine("Something went wrong... skipping..." + e.Message);
            //}
        }


        public static double[] get_boundingbox(Structure s)
        {
            if (s.MeshGeometry == null)
            {
                return new double[] { 0, 0, 0, 0, 0, 0 };
            }

            System.Windows.Media.Media3D.Rect3D b = s.MeshGeometry.Bounds;
            return new double[]{
                b.X, b.X+b.SizeX,
                b.Y, b.Y+b.SizeY,
                b.Z, b.Z+b.SizeZ};
        }

        public static double[] calculate_boundingbox(Structure s)
        {
            if (s.MeshGeometry == null)
                return new double[] { 0, 0, 0, 0, 0, 0 };

            //
            // NOTE: No need to calculate! s.MeshGeometry.Bounds has the info
            //

            // calculate the bounding box
            double[] bbox = new double[6];
            {
                double minx = 100000000000;
                double maxx = -100000000000;
                double miny = 100000000000;
                double maxy = -100000000000;
                double minz = 100000000000;
                double maxz = -100000000000;

                foreach (System.Windows.Media.Media3D.Point3D point in s.MeshGeometry.Positions)
                {
                    maxx = Math.Max(maxx, point.X);
                    minx = Math.Min(minx, point.X);

                    maxy = Math.Max(maxy, point.Y);
                    miny = Math.Min(miny, point.Y);

                    maxz = Math.Max(maxz, point.Z);
                    minz = Math.Min(minz, point.Z);
                }

                // save bounding box
                bbox[0] = minx;
                bbox[1] = maxx;
                bbox[2] = miny;
                bbox[3] = maxy;
                bbox[4] = minz;
                bbox[5] = maxz;
            }

            return bbox;
        }


        public static void export_image(VMS.TPS.Common.Model.API.Image img, string dir, string name, bool export_eclipse_internal_pixel_data = false)
        {
            string fn = $"export_image(img={_Id(img)},dir={dir},name={name})";
            print(fn);

            // image size is too small, the size if irreg cases are [1x1x1]
            if (img.XSize < 10)
            {
                global_variables.log(string.Format("invalid image size:{0},{1},{2}", img.XSize, img.YSize, img.ZSize));
                return;
            }

            export_image_mhd(img, dir, name, export_eclipse_internal_pixel_data);
            write_image_info(img, dir);
        }

        public static void write_image_info(VMS.TPS.Common.Model.API.Image img, string dir)
        {
            string fn = $"write_image_info(img={_Id(img)},dir={dir})";
            print(fn);

            string info_file = filesystem.join(dir, "info.txt");

            // skip if file exists
            if (filesystem.file_exists(info_file))
                return;

            StringBuilder sb = new StringBuilder();
            {
                sb.AppendLine("Id=" + img.Id);
                sb.AppendLine("Comment=" + img.Comment);
                sb.AppendLine("CreationDateTime=" + img.CreationDateTime.ToString());
                sb.AppendLine("DisplayUnit=" + img.DisplayUnit);
                sb.AppendLine("FOR=" + img.FOR);
                sb.AppendLine("ImagingOrientation=" + img.ImagingOrientation);
                sb.AppendLine("HasUserOrigin=" + img.HasUserOrigin.ToString());
                sb.AppendLine("HistoryDateTime=" + img.HistoryDateTime.ToString());
                sb.AppendLine("HistoryUserName=" + img.HistoryUserName.ToString());
                sb.AppendLine("Series=" + img.Series);
                sb.AppendLine("Level=" + img.Level.ToString());
                sb.AppendLine("Window=" + img.Window.ToString());
            }
            System.IO.File.WriteAllText(info_file, sb.ToString());

            // log
            if (global_variables.make_export_log)
                export_log.new_img_info_file_created(info_file);
        }

        public static string _str(double[] list, string delim)
        {
            string[] strlist = list.Select(x => x.ToString()).ToArray();
            return String.Join(delim, strlist);
        }

        public static void export_image_mhd(VMS.TPS.Common.Model.API.Image img, string dir, string name, bool export_eclipse_internal_pixel_data = false)
        {
            string fn = $"export_image_mhd(img={_Id(img)},dir={dir}, name={name}, export_eclipse_internal_pixel_data={export_eclipse_internal_pixel_data})";
            print(fn);

            int[] size = new int[] { img.XSize, img.YSize, img.ZSize };
            //print(string.Format("size=({0},{1},{2})", size[0], size[1], size[2]));

            double[] spacing = new double[] { img.XRes, img.YRes, img.ZRes };
            VVector org = img.Origin;
            VVector vX = img.XDirection;
            VVector vY = img.YDirection;
            VVector vZ = img.ZDirection;
            double[] direction = new double[9]
            {
                vX[0], vX[1], vX[2],
                vY[0], vY[1], vY[2],
                vZ[0], vZ[1], vZ[2]
            };

            //print("num of pixels = {0}", size[0] * size[1] * size[2]);
            string file_mhd = name + ".mhd";
            string file_mha = name + ".mha";

            string path_mhd = filesystem.join(dir, file_mhd);
            string path_mha = filesystem.join(dir, file_mha);

            if (image_exist(path_mhd))
            {
                print("Image found. so, skipping..." + path_mhd);
                return;
            }

            ////////////////
            // save header
            if (!filesystem.file_exists(path_mhd))
            {
                StringBuilder sb = new StringBuilder();
                {
                    sb.AppendLine("ObjectType = Image");
                    sb.AppendLine("NDims = 3");
                    sb.AppendLine("BinaryDataByteOrderMSB = False");
                    sb.AppendLine("CompressedData = True");
                    sb.AppendLine("TransformMatrix = " + _str(direction, " "));
                    sb.AppendLine(string.Format("Offset = {0} {1} {2}", org.x, org.y, org.z));
                    sb.AppendLine("CenterOfRotation = 0 0 0");
                    sb.AppendLine(string.Format("ElementSpacing = {0} {1} {2}", spacing[0], spacing[1], spacing[2]));
                    sb.AppendLine(string.Format("DimSize = {0} {1} {2}", size[0], size[1], size[2]));
                    sb.AppendLine("AnatomicalOrientation = ???");
                    sb.AppendLine(string.Format("ElementSize = {0} {1} {2}", spacing[0], spacing[1], spacing[2]));
                    // Set correct ElementType based on export mode
                    if (export_eclipse_internal_pixel_data)
                        sb.AppendLine("ElementType = MET_INT");
                    else
                        sb.AppendLine("ElementType = MET_SHORT");
                    sb.AppendLine("ElementDataFile = " + file_mha);

                    print("saving..." + path_mhd);
                    System.IO.File.WriteAllText(path_mhd, sb.ToString());
                }
                // log
                if (global_variables.make_export_log)
                    export_log.new_img_mhd_file_created(path_mhd);
            }

            /////////////////////
            // save pixel data
            int[,] buffer = new int[size[0], size[1]];

            uint[] sz = new uint[] { (uint)size[0], (uint)size[1], (uint)size[2] };
            itk.simple.VectorUInt32 itkSize = new itk.simple.VectorUInt32(sz);
            itk.simple.VectorDouble itkSpacing = new itk.simple.VectorDouble(spacing);
            itk.simple.VectorDouble itkOrigin = new itk.simple.VectorDouble(_V2D(org));
            itk.simple.VectorDouble itkDirection = new itk.simple.VectorDouble(direction);

            itk.simple.Image itkImage = null;
            if(export_eclipse_internal_pixel_data)
                itkImage = new itk.simple.Image(itkSize, itk.simple.PixelIDValueEnum.sitkInt32);
            else
                itkImage = new itk.simple.Image(itkSize, itk.simple.PixelIDValueEnum.sitkInt16);

            itkImage.SetSpacing(itkSpacing);
            itkImage.SetOrigin(itkOrigin);
            itkImage.SetDirection(itkDirection);

            // Use appropriate buffer type based on export mode
            if (export_eclipse_internal_pixel_data)
            {
                Int32[] bufferSave = new Int32[size[0] * size[1] * size[2]];
                for (uint z = 0; z < size[2]; z++)
                {
                    // get a slice 
                    img.GetVoxels((int)z, buffer);
                    Console.Write(z + ".");

                    // copy to raw pixel data buffer (Int32)
                    for (uint y = 0; y < size[1]; y++)
                    {
                        for (uint x = 0; x < size[0]; x++)
                        {
                            bufferSave[z * (size[0] * size[1]) + y * (size[0]) + x] = buffer[x, y];
                        } // x
                    } // Y
                } // Z

                // copy to itk image memory
                int length = size[0] * size[1] * size[2];
                System.Runtime.InteropServices.Marshal.Copy(bufferSave, 0, itkImage.GetBufferAsInt32(), length);
            }
            else
            {
                // Optimize: Calculate linear mapping constants from middle slice
                // VoxelToDisplayValue is a linear mapping: display = a * voxel + b
                double slope = 1.0;
                double intercept = 0.0;
                bool useDirectConversion = false; // Flag: if true, use VoxelToDisplayValue() for all pixels
                {
                    int middleZ = (int)(size[2] / 2);
                    img.GetVoxels(middleZ, buffer);
                    
                    // Find min and max pixel values in middle slice
                    int pixelMin = int.MaxValue;
                    int pixelMax = int.MinValue;
                    for (int y = 0; y < size[1]; y++)
                    {
                        for (int x = 0; x < size[0]; x++)
                        {
                            int pixelValue = buffer[x, y];
                            if (pixelValue < pixelMin) pixelMin = pixelValue;
                            if (pixelValue > pixelMax) pixelMax = pixelValue;
                        }
                    }
                    
                    // Check if all pixels have the same value
                    if (pixelMax == pixelMin)
                    {
                        // All pixels have same value, use direct conversion
                        useDirectConversion = true;
                        print($"All pixels in middle slice (z={middleZ}) have same value ({pixelMin}). Using VoxelToDisplayValue() for all pixels.");
                    }
                    else
                    {
                        // Get display values for min and max
                        double displayMin = img.VoxelToDisplayValue(pixelMin);
                        double displayMax = img.VoxelToDisplayValue(pixelMax);
                        
                        // Calculate slope and intercept: display = a * voxel + b
                        slope = (displayMax - displayMin) / (pixelMax - pixelMin);
                        intercept = displayMin - slope * pixelMin;
                        
                        print($"Linear mapping calculated from middle slice (z={middleZ}): slope={slope}, intercept={intercept}, pixelRange=[{pixelMin}, {pixelMax}], displayRange=[{displayMin}, {displayMax}]");
                    }
                }
                
                Int16[] bufferSave = new Int16[size[0] * size[1] * size[2]];
                for (uint z = 0; z < size[2]; z++)
                {
                    // get a slice 
                    img.GetVoxels((int)z, buffer);
                    Console.Write(z + ".");

                    // copy to Int16 buffer (convert to display value)
                    if (useDirectConversion)
                    {
                        // Use direct conversion for all pixels
                        for (uint y = 0; y < size[1]; y++)
                        {
                            for (uint x = 0; x < size[0]; x++)
                            {
                                Int16 value = (Int16)img.VoxelToDisplayValue(buffer[x, y]);
                                bufferSave[z * (size[0] * size[1]) + y * (size[0]) + x] = value;
                            } // x
                        } // Y
                    }
                    else
                    {
                        // Use linear transformation: display = slope * voxel + intercept
                        for (uint y = 0; y < size[1]; y++)
                        {
                            for (uint x = 0; x < size[0]; x++)
                            {
                                double displayValue = slope * buffer[x, y] + intercept;
                                Int16 value = (Int16)Math.Round(displayValue);
                                bufferSave[z * (size[0] * size[1]) + y * (size[0]) + x] = value;
                            } // x
                        } // Y
                    }
                } // Z

                // copy to itk image memory
                int length = size[0] * size[1] * size[2];
                System.Runtime.InteropServices.Marshal.Copy(bufferSave, 0, itkImage.GetBufferAsInt16(), length);
            }

            // Save the image to a file (always compressed)
            simpleitk.save_itkimage(itkImage, path_mha, useCompression: true);
            print("Images created and saved successfully. path=" + path_mha);

            // log
            if (global_variables.make_export_log)
                export_log.new_img_mha_file_created(path_mha);
        }


        public static void write_structureset_info(StructureSet sset, string dir)
        {
            string fn = $"write_structureset_info(sset={_Id(sset)},dir={dir})";
            print(fn);

            string info_file = filesystem.join(dir, "info.txt");

            // skip if file exists
            if (filesystem.file_exists(info_file))
                return;

            StringBuilder sb = new StringBuilder();
            {
                sb.AppendLine("Id=" + sset.Id);
                sb.AppendLine("Comment=" + sset.Comment);
                sb.AppendLine("HistoryDateTime=" + sset.HistoryDateTime.ToString());
                sb.AppendLine("HistoryUserName=" + sset.HistoryUserName);
                sb.AppendLine("Image.Id=" + sset.Image.Id);
                sb.AppendLine("UID=" + sset.UID);
            }
            System.IO.File.WriteAllText(info_file, sb.ToString());

            // log
            if (global_variables.make_export_log)
                export_log.new_sset_info_file_created(info_file);
        }

        public static void export_structureset(StructureSet sset, string dir)
        {
            string fn = $"export_structureset(sset={_Id(sset)},dir={dir})";
            print(fn);

            write_structureset_info(sset, dir);

            foreach (Structure s in sset.Structures)
            {
                // skip empty structures
                if (s.IsEmpty)
                {
                    global_variables.log("skipping empty structure:" + s.Id);
                    continue;
                }

                // skip markers                
                if (s.DicomType == "MARKER")
                {
                    global_variables.log("skipping MARKER structure:" + s.Id);
                    continue;
                }

                if (string.Format("{0}", s.Volume) == "NaN")
                {
                    global_variables.log("skipping zero volume structure:" + s.Id);
                    continue;
                }

                // skip dose contours
                if (s.Id.ToLower().Contains("dose"))
                {
                    global_variables.log("skipping dose structure:" + s.Id);
                    continue;
                }

                if (s.Id.ToLower().Contains("cgy"))
                {
                    global_variables.log("skipping structure with cgy in Id:" + s.Id);
                    continue;
                }

                if (s.Id.ToLower().Contains("%"))
                {
                    global_variables.log("skipping structure with % in Id:" + s.Id);
                    continue;
                }

                if (s.Id.ToLower().Contains("prv"))
                {
                    global_variables.log("skipping structure with 'prv' in Id:" + s.Id);
                    continue;
                }

                if (s.Id.ToLower().Contains("skin"))
                {
                    global_variables.log("skipping structure with 'skin' in Id:" + s.Id);
                    continue;
                }

                if (s.Id.ToLower().Contains("ring"))
                {
                    global_variables.log("skipping structure with 'ring' in Id:" + s.Id);
                    continue;
                }


                print(string.Format("export_structure_as_mask_image({0})", s.Id));
                export_structure_as_mask_image(sset.Image, s, dir, filesystem.encode_path(s.Id));
            }
        }

        public static void export_structureset_with_sid_like(StructureSet sset, string sid_like, string dir)
        {
            string fn = $"export_structureset_with_sid_like(sset={_Id(sset)},sid_like={sid_like}, dir={dir})";
            print(fn);

            write_structureset_info(sset, dir);

            foreach (Structure s in sset.Structures)
            {
                // skip empty structures
                if (s.IsEmpty)
                {
                    global_variables.log("skipping empty structure:" + s.Id);
                    continue;
                }

                // skip markers                
                if (s.DicomType == "MARKER")
                {
                    global_variables.log("skipping MARKER structure:" + s.Id);
                    continue;
                }

                if (string.Format("{0}", s.Volume) == "NaN")
                {
                    global_variables.log("skipping zero volume structure:" + s.Id);
                    continue;
                }

                // skip dose contours
                if (s.Id.ToLower().Contains("dose"))
                {
                    global_variables.log("skipping dose structure:" + s.Id);
                    continue;
                }

                if (s.Id.ToLower().Contains("cgy"))
                {
                    global_variables.log("skipping structure with cgy in Id:" + s.Id);
                    continue;
                }

                if (s.Id.ToLower().Contains("%"))
                {
                    global_variables.log("skipping structure with % in Id:" + s.Id);
                    continue;
                }

                if (s.Id.ToLower().Contains("prv"))
                {
                    global_variables.log("skipping structure with 'prv' in Id:" + s.Id);
                    continue;
                }

                if (s.Id.ToLower().Contains("skin"))
                {
                    global_variables.log("skipping structure with 'skin' in Id:" + s.Id);
                    continue;
                }

                // only exports if the structure id is like the givien sid_like.
                if (s.Id.ToLower().Contains(sid_like.Trim().ToLower()))
                {
                    print(string.Format("export_structure_as_mask_image({0})", s.Id));
                    export_structure_as_mask_image(sset.Image, s, dir, filesystem.encode_path(s.Id));
                }
            }
        }

        public static bool image_exist(string path_mhd)
        {
            if (!System.IO.File.Exists(path_mhd))
            {
                print("mhd not found: " + path_mhd);
                return false;
            }

            string path_raw = path_mhd.Replace(".mhd", ".raw");
            string path_zraw = path_mhd.Replace(".mhd", ".zraw");
            string path_mha = path_mhd.Replace(".mhd", ".mha");


            if (System.IO.File.Exists(path_raw))
            {
                Console.Write("Image exist (mhd & raw), skipping...;" + path_mhd);
                return true;
            }
            else if (System.IO.File.Exists(path_zraw))
            {
                Console.Write("Image exist (mhd & zraw), skipping...;" + path_mhd);
                return true;
            }
            else if (System.IO.File.Exists(path_mha))
            {
                Console.Write("Image exist (mhd & mha), skipping...;" + path_mhd);
                return true;
            }
            else
            {
                print("Image not found:" + path_mhd);
                return false;
            }
        }

        public static void write_structure_info(Structure s, string path_info)
        {
            string fn = $"write_structure_info(s={_Id(s)}, path_info={path_info})";
            print(fn);

            // skip if file exists
            if (filesystem.file_exists(path_info))
                return;

            double[] bbox = get_boundingbox(s);
            StringBuilder sb = new StringBuilder();
            {
                sb.AppendLine(string.Format("bbox={0},{1},{2},{3},{4},{5}", bbox[0], bbox[1], bbox[2], bbox[3], bbox[4], bbox[5]));
                sb.AppendLine(string.Format("volume={0}", s.Volume));
                sb.AppendLine(string.Format("NumberOfSeparateParts={0}", s.GetNumberOfSeparateParts()));
                sb.AppendLine("HistoryDateTime=" + s.HistoryDateTime.ToString());
                sb.AppendLine("HistoryUserName=" + s.HistoryUserName);
                sb.AppendLine("DicomType=" + s.DicomType);
                sb.AppendLine("Color=" + s.Color.ToString());
                sb.AppendLine("HasSegment=" + s.HasSegment.ToString());
                sb.AppendLine("IsHighResolution=" + s.IsHighResolution.ToString());
                sb.AppendLine("Comment=" + s.Comment);
                sb.AppendLine("ROINumber=" + s.ROINumber.ToString());
                if (s.MeshGeometry == null)
                    sb.AppendLine("MeshGeometry.Bounds=0,0,0,0,0,0");
                else
                    sb.AppendLine("MeshGeometry.Bounds=" + s.MeshGeometry.Bounds.ToString());
            }
            System.IO.File.WriteAllText(path_info, sb.ToString());

            // log
            if (global_variables.make_export_log)
                export_log.new_structure_info_file_created(path_info);

        }

        public static VVector cross(VVector vectorA, VVector vectorB)
        {
            return new VVector(
           vectorA.y * vectorB.z - vectorA.z * vectorB.y,
           vectorA.z * vectorB.x - vectorA.x * vectorB.z,
           vectorA.x * vectorB.y - vectorA.y * vectorB.x);
        }

        private static itk.simple.Image create_itkimage_of_structure(Structure s, VMS.TPS.Common.Model.API.Image img)
        {
            string fn = $"create_itkImage_of_structure(s={s.Id}, img={img.Id})";

            int[] size = new int[] { img.XSize, img.YSize, img.ZSize };
            double[] spacing = new double[] { img.XRes, img.YRes, img.ZRes };
            VVector vX = img.XDirection;
            VVector vY = img.YDirection;
            VVector vZ = img.ZDirection;
            double[] direction = new double[9]
            {
                vX[0], vX[1], vX[2],
                vY[0], vY[1], vY[2],
                vZ[0], vZ[1], vZ[2]
            };

            double[] org = new double[] { img.Origin[0], img.Origin[1], img.Origin[2] };

            uint[] sz = new uint[] { (uint)size[0], (uint)size[1], (uint)size[2] };
            itk.simple.VectorUInt32 itkSize = new itk.simple.VectorUInt32(sz);
            itk.simple.VectorDouble itkSpacing = new itk.simple.VectorDouble(spacing);
            itk.simple.VectorDouble itkOrigin = new itk.simple.VectorDouble(org);
            itk.simple.VectorDouble itkDirection = new itk.simple.VectorDouble(direction);
            itk.simple.Image itkImage = new itk.simple.Image(itkSize, itk.simple.PixelIDValueEnum.sitkUInt8);
            itkImage.SetSpacing(itkSpacing);
            itkImage.SetOrigin(itkOrigin);
            itkImage.SetDirection(itkDirection);

            // create pixel buffer
            byte[] buffer = create_pixel_buffer_of_structure(s, img);

            // copy to itk image memory
            print($"Copying memory to sitkImage buffer, length={buffer.Length}. fn={fn}");
            System.Runtime.InteropServices.Marshal.Copy(buffer, 0, itkImage.GetBufferAsUInt8(), buffer.Length);

            return itkImage;
        }


        private static double[] _V2D(VVector v)
        {
            return new double[3] { v[0], v[1], v[2] };
        }

        private static byte[] create_pixel_buffer_of_structure(Structure s, VMS.TPS.Common.Model.API.Image img)
        {
            string fn = $"create_pixel_buffer_of_structure(s={s.Id}, img={img.Id})";
            print(fn);

            // img orgin, size, and pixel size
            double[] org = _V2D(img.Origin);
            int[] size = new int[] { img.XSize, img.YSize, img.ZSize };
            double[] pxsz = new double[] { img.XRes, img.YRes, img.ZRes };
            // unit direction vetor
            double[] ux = _V2D(img.XDirection);
            double[] uy = _V2D(img.YDirection);
            double[] uz = _V2D(img.ZDirection);

            // temp memory
            VVector point = new VVector();
            byte[] buffer = new byte[size[0] * size[1] * size[2]];
            double[] d = new double[3];

            // bounding box of the structure in w.
            double[] bbox = get_boundingbox(s);


            int count_in = 0;
            for (int z = 0; z < size[2]; z++)
            {
                Console.Write(z.ToString() + ".");

                for (int y = 0; y < size[1]; y++)
                {
                    for (int x = 0; x < size[0]; x++)
                    {
                        // offset in image
                        d[0] = x * pxsz[0];
                        d[1] = y * pxsz[1];
                        d[2] = z * pxsz[2];

                        // point in w (the dirrection vectors are used to find the offset in w)
                        point[0] = org[0] + d[0] * ux[0] + d[1] * ux[1] + d[2] * ux[2];
                        point[1] = org[1] + d[0] * uy[0] + d[1] * uy[1] + d[2] * uy[2];
                        point[2] = org[2] + d[0] * uz[0] + d[1] * uz[1] + d[2] * uz[2];

                        int i = z * size[0] * size[1] + y * size[0] + x;

                        if (point.x < bbox[0] || point.x > bbox[1] ||
                        point.y < bbox[2] || point.y > bbox[3] ||
                            point.z < bbox[4] || point.z > bbox[5])
                        {
                            buffer[i] = 0;
                        }
                        else
                        {
                            byte value = (s.IsPointInsideSegment(point)) ? (byte)1 : (byte)0;
                            buffer[i] = value;
                            count_in += 1;
                        }
                    } // x
                } // Y
            } // Z

            print($"number of pixels of value 1 = {count_in}. fn={fn}");
            return buffer;
        }

        public static void export_structure_as_mask_image(VMS.TPS.Common.Model.API.Image img, Structure s, string dir, string name)
        {
            string fn = $"export_structure_as_mask_image(img={img.Id}, s={s.Id}, dir={dir}, name={name})";
            print(fn);

            ////////////////////////////
            // write structure info file
            // bounding box
            string file_info = name + ".info";
            string path_info = filesystem.join(dir, file_info);
            write_structure_info(s, path_info);

            ////////////////
            // write image
            string file_raw = name + ".raw";
            string file_mhd = name + ".mhd";
            string file_mha = name + ".mha";
            string path_mhd = filesystem.join(dir, file_mhd);
            string path_mha = filesystem.join(dir, file_mha);

            // check all images are already present. If so, skip.
            if (image_exist(path_mhd))
            {
                helper.print($"Image exists, skipping... s={s.Id}. fn={fn}");
                return;
            }


            ////////////////
            // write mhd
            if (!filesystem.file_exists(path_mhd))
            {
                int[] size = new int[] { img.XSize, img.YSize, img.ZSize };
                double[] spacing = new double[] { img.XRes, img.YRes, img.ZRes };
                double[] org = new double[] { img.Origin[0], img.Origin[1], img.Origin[2] };
                VVector vX = img.XDirection;
                VVector vY = img.YDirection;
                VVector vZ = img.ZDirection;
                double[] direction = new double[9]
                {
                vX[0], vX[1], vX[2],
                vY[0], vY[1], vY[2],
                vZ[0], vZ[1], vZ[2]
                };

                StringBuilder sb2 = new StringBuilder();
                sb2.AppendLine("ObjectType = Image");
                sb2.AppendLine("NDims = 3");
                sb2.AppendLine("BinaryDataByteOrderMSB = False");
                sb2.AppendLine("CompressedData = True");
                sb2.AppendLine("TransformMatrix = " + _str(direction, " "));
                sb2.AppendLine(string.Format("Offset = {0} {1} {2}", org[0], org[1], org[2]));
                sb2.AppendLine("CenterOfRotation = 0 0 0");
                sb2.AppendLine(string.Format("ElementSpacing = {0} {1} {2}", spacing[0], spacing[1], spacing[2]));
                sb2.AppendLine(string.Format("DimSize = {0} {1} {2}", size[0], size[1], size[2]));
                sb2.AppendLine("AnatomicalOrientation = ???");
                sb2.AppendLine(string.Format("ElementSize = {0} {1} {2}", spacing[0], spacing[1], spacing[2]));
                sb2.AppendLine("ElementType = MET_UCHAR");
                sb2.AppendLine("ElementDataFile = " + file_raw);
                System.IO.File.WriteAllText(path_mhd, sb2.ToString());

                // log
                if (global_variables.make_export_log)
                    export_log.new_structure_mhd_file_created(path_mhd);
            }

            /////////////////////
            // save pixel data (this assumes the bbox is correct... and thus MeshGeometry is correct.
            itk.simple.Image itkImage = create_itkimage_of_structure(s, img);
            if (itkImage == null)
            {
                error($"itkImage is null. fn={fn}");
                return;
            }


            // Save the image to a file (always compressed)
            simpleitk.save_itkimage(itkImage, path_mha, useCompression: true);
            //save_itkimage(itkImage, path_mhd);

            if (global_variables.make_export_log)
                export_log.new_structure_mha_file_created(path_mha);

        }



        //public static string[] get_StructureSetSerList_with_StructureName(string name)
        //{
        //    try
        //    {
        //        List<string> col_headers = new List<string>();
        //        List<string> lines = new List<string>();

        //        variandb.variansystem sysdb = new variandb.variansystem(global_variables.db_server);
        //        sysdb.open_connection();
        //        {
        //            string sql = string.Format(@"select DISTINCT s.StructureSetSer from Structure s where s.StructureId = '{0}';", name);

        //            System.Data.SqlClient.SqlDataReader dr = sysdb.exe_sql_cmd(sql);

        //            while (dr.Read())
        //            {
        //                // get the column header                   
        //                if (col_headers.Count == 0)
        //                {
        //                    for (int i = 0; i < dr.FieldCount; i++)
        //                    {
        //                        string key = dr.GetName(i);
        //                        col_headers.Add(key);
        //                    }

        //                    lines.Add(string.Join(",", col_headers));
        //                }

        //                // get the values
        //                List<string> values = new List<string>();
        //                for (int i = 0; i < dr.FieldCount; i++)
        //                {
        //                    string val = string.Format("{0}", dr[i]);
        //                    values.Add(val);
        //                }
        //                lines.Add(string.Join(",", values));
        //            }
        //        }
        //        sysdb.close_connection();

        //        return lines.ToArray();
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }
        //}
    }
}
