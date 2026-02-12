using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace esapi
{
    public static class esapi
    {
        public static Image image_of_id(string id, Patient pt)
        {
            foreach (Study study in pt.Studies)
            {
                foreach (Series srs in study.Series)
                {
                    foreach (Image image in srs.Images)
                    {
                        if (image.Id == id)
                            return image;
                    }
                }
            }

            return null;
        }

        public static Image image_of_id_FOR(string id, string FOR, Patient pt)
        {
            foreach (Study study in pt.Studies)
            {
                foreach (Series srs in study.Series)
                {
                    foreach (Image image in srs.Images)
                    {
                        if (image.Id == id && image.FOR == FOR)
                            return image;
                    }
                }
            }

            return null;
        }

        public static Image first_image_of_id(string id, Patient pt)
        {
            foreach (Study study in pt.Studies)
            {
                foreach (Series srs in study.Series)
                {
                    foreach (Image image in srs.Images)
                    {
                        if (image.Id == id)
                            return image;
                    }
                }
            }

            return null;
        }




        public static StructureSet sset_of_id(string id, Patient pt)
        {
            foreach (StructureSet sset in pt.StructureSets)
            {
                if (sset.Id == id)
                    return sset;
            }

            return null;
        }

        public static StructureSet[] sset_list_of_id(string id, Patient pt)
        {
            List<StructureSet> list = new List<StructureSet>();
            foreach (StructureSet sset in pt.StructureSets)
            {
                if (sset.Id == id)
                    list.Add(sset);
            }

            return list.ToArray();
        }


        public static StructureSet sset_of_uid(string uid, Patient pt)
        {
            foreach (StructureSet sset in pt.StructureSets)
            {
                if (sset.UID == uid)
                    return sset;
            }

            return null;
        }



        public static List<StructureSet> sset_list_of_image_id(string id, Patient pt)
        {
            List<StructureSet> list = new List<StructureSet>();
            foreach (StructureSet sset in pt.StructureSets)
            {
                if (sset.Image.Id == id)
                    list.Add(sset);
            }

            return list;
        }

        public static List<StructureSet> sset_list_of_image_id_FOR(string image_id, string image_FOR, Patient pt)
        {
            List<StructureSet> list = new List<StructureSet>();
            foreach (StructureSet sset in pt.StructureSets)
            {
                if (sset.Image.Id == image_id && sset.Image.FOR == image_FOR)
                    list.Add(sset);
            }

            return list;
        }

        public static Structure find_or_add_s(string DicomType, string Id, StructureSet sset, bool case_sensitive= false)
        {
            Structure s = s_of_id(Id, sset, case_sensitive);
            if (s != null)
                return s;

            if (!sset.CanAddStructure(DicomType, Id))
            {
                helper.error("Cannot add a structure to the structureset!");
                return null;
            }

            return sset.AddStructure(DicomType, Id);
        }


        public static void s_clear_all_slices(Structure s, int num_slices)
        {
            for (int z = 0; z < num_slices; z++)
            {
                s.ClearAllContoursOnImagePlane(z);
            }
        }

        public static DoseValue s2D(string str)
        {
            string s_lower = str.ToLower().Trim();
            if (s_lower.Contains('%'))
            {
                double value = Convert.ToDouble(s_lower.Replace("%", ""));
                return new DoseValue(value, DoseValue.DoseUnit.Percent);
            }
            else if (s_lower.Contains("cgy"))
            {
                double value = Convert.ToDouble(s_lower.Replace("cgy", ""));
                return new DoseValue(value, DoseValue.DoseUnit.cGy);
            }
            else if (s_lower.Contains("gy"))
            {
                double value = Convert.ToDouble(s_lower.Replace("gy", ""));
                return new DoseValue(value, DoseValue.DoseUnit.Gy);
            }
            else
            {
                throw new Exception("Invalid dose string:" + str);
            }
        }
        public static Structure s_of_id(string id, StructureSet sset, bool case_sensitive = true)
        {
            if (case_sensitive)
            {
                foreach (Structure s in sset.Structures)
                    if (s.Id == id)
                        return s;
            }
            else
            {
                foreach (Structure s in sset.Structures)
                    if (s.Id.ToLower().Trim() == id.ToLower().Trim())
                        return s;
            }
            return null;
        }

        public static Structure s_of_type_external(StructureSet sset)
        {
            foreach (Structure s in sset.Structures)
            {
                if (s.DicomType == "EXTERNAL")
                    return s;
            }

            return null;
        }

        public static ExternalPlanSetup[] ext_ps_of_sset(StructureSet sset, bool exclude_if_rejected = true)
        {
            List<ExternalPlanSetup> list = new List<ExternalPlanSetup>();
            foreach (Course c in sset.Patient.Courses)
            {
                foreach (ExternalPlanSetup ps in c.ExternalPlanSetups)
                {
                    if (exclude_if_rejected && ps.ApprovalStatus == PlanSetupApprovalStatus.Rejected)
                        continue;

                    if (ps.StructureSet == null) // IRREG plan
                        continue;

                    if (ps.StructureSet.Id == sset.Id)
                        list.Add(ps);
                }
            }
            return list.ToArray();
        }

        public static void s_load_contour_data_from_cont_json_file(Structure s, string cont_json_file)
        {
            string json = System.IO.File.ReadAllText(cont_json_file);
            SliceContours[] conts = JsonConvert.DeserializeObject<SliceContours[]>(json);
            foreach (SliceContours c_list in conts)
            {
                //helper.print($"slice: {c_list.Slice}");

                int z = c_list.Slice;

                List<VVector[]> vVectors = new List<VVector[]>();
                foreach (Contour contour in c_list.Contours)
                {

                    List<List<double>> points = contour.Points;
                    bool hole = contour.Hole;

                    List<VVector> vectors = new List<VVector>();
                    foreach (List<double> point in points)
                    {
                        VVector v = new VVector();
                        v[0] = point[0];
                        v[1] = point[1];
                        v[2] = point[2];
                        vectors.Add(v);
                    }

                    if (!hole)
                        s.AddContourOnImagePlane(vectors.ToArray(), z);
                    else
                        s.SubtractContourOnImagePlane(vectors.ToArray(), z);
                }
            }
        }

        public static Course cs_add(string Id, Patient pt)
        {
            if (pt.CanAddCourse())
            {
                Course cs = pt.AddCourse();
                cs.Id = Id;
                return cs;
            }
            else
            {
                helper.error($"Cannot add a new course of Id={Id}");
                return null;
            }
        }


        public static PlanSetup ps_of_id(string ps_Id, Patient pt)
        {
            foreach (Course cs in pt.Courses)
            {
                foreach (PlanSetup ps in cs.PlanSetups)
                {
                    if (ps.Id.Trim().ToLower() == ps_Id.Trim().ToLower())
                        return ps;
                }
            }

            return null;
        }

        public static int remove_ps(string ps_Id, Patient pt)
        {
            PlanSetup ps = ps_of_id(ps_Id, pt);
            if (ps == null)
                return 0; // nothing to remove

            // remove plan setup
            ps.Course.RemovePlanSetup(ps);

            return 1;
        }

        public static Structure user_select_s_of_sset(StructureSet sset)
        {
            Structure[] s_list = sset.Structures.ToArray();
            for (int i = 0; i < s_list.Length; i++)
            {
                helper.print($"{i} - {s_list[i].Id} - {s_list[i].Volume.ToString("0.0")}");
            }
            int selection = Convert.ToInt32(Console.ReadLine());

            if (selection == -1)
                return null;

            return s_list[selection];
        }

        public static ExternalPlanSetup user_select_ext_ps_of_sset(StructureSet sset)
        {
            ExternalPlanSetup[] ps_list = ext_ps_of_sset(sset);
            for (int n = 0; n < ps_list.Length; n++)
            {
                ExternalPlanSetup plan = ps_list[n];
                helper.print($"{n} - {plan.Id} - {plan.ApprovalStatus}");
            }
            int selection = Convert.ToInt32(Console.ReadLine());
            if (selection == -1) return null;
            else return ps_list[selection];
        }





        public static Course cs_of_id(string Id, Patient pt)
        {
            foreach (Course cs in pt.Courses)
            {
                if (cs.Id.Trim().ToLower() == Id.Trim().ToLower())
                    return cs;
            }

            return null;
        }

        public static Course find_or_add_cs(string Id, Patient pt)
        {
            Course cs = cs_of_id(Id, pt);
            if (cs != null)
                return cs;

            if (!pt.CanAddCourse())
            {
                helper.error("Cannot add a course to the patient!");
                return null;
            }

            Course new_cs = pt.AddCourse();
            new_cs.Id = Id;

            return new_cs;
        }
        public static ExternalPlanSetup find_or_add_ext_ps(string Id, StructureSet sset, Course cs)
        {
            PlanSetup ps = ps_of_id(Id, cs.Patient);
            if (ps != null)
                return (ExternalPlanSetup)ps;

            if (!cs.CanAddPlanSetup(sset))
            {
                helper.error($"Cannot add a plansetup to the course(Id={cs.Id} with the structureset (Id={sset.Id})!");
                return null;
            }

            ExternalPlanSetup new_ps = cs.AddExternalPlanSetup(sset);
            new_ps.Id = Id;

            return new_ps;
        }

        public static Beam bm_of_ps(string id, PlanSetup ps, bool case_sensitive = true)
        {
            if (case_sensitive)
            {
                foreach (Beam bm in ps.Beams)
                    if (bm.Id.Trim() == id.Trim())
                        return bm;
            }
            else
            {
                foreach (Beam bm in ps.Beams)
                    if (bm.Id.ToLower().Trim() == id.ToLower().Trim())
                        return bm;
            }
            return null;
        }

        public static Beam add_setup_beam(ExternalPlanSetup ps, string beam_id, double g, VVector isocenter, ExternalBeamMachineParameters machine)
        {
            VRect<double> jawPositions = new VRect<double>(-50, -50, 50, 50);
            double collimatorAngle = 0.0;
            double patientSupportAngle = 0.0;
            Beam b = ps.AddSetupBeam(machine, jawPositions, collimatorAngle, g, patientSupportAngle, isocenter);
            b.Id = beam_id;

            return b;
        }

        public static Beam add_bony_drr(Beam b)
        {
            // Bone window DRR
            double drrSize = 500.0; //mm
            double weight = 1.0;
            double ctFrom = 125; // HU
            double ctTo = 1000; // HU
            double geoFrom = -1000; // mm
            double geoTo = 1000; // mm
            DRRCalculationParameters drrParam = new DRRCalculationParameters(drrSize, weight, ctFrom, ctTo, geoFrom, geoTo);
            b.CreateOrReplaceDRR(drrParam);

            return b;
        }

    }
}
