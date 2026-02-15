select top 10 * from Patient
where PatientSer = 34946

select * from PlanSetup as ps, Course as cs, Patient as pt
where ps.CourseSer = cs.CourseSer
and cs.PatientSer = pt.PatientSer
and LOWER(ps.PlanSetupId) LIKE 'bladder%'
and ps.Status = 'TreatApproval'
order by ps.HstryTimeStamp desc


select pt.PatientId, cs.CourseId, ps.PlanSetupId, ps.CreationDate
from PlanSetup as ps, Course as cs, Patient as pt
where ps.CourseSer = cs.CourseSer
and cs.PatientSer = pt.PatientSer
and LOWER(ps.PlanSetupId) LIKE '_bladder_art'
order by ps.HstryTimeStamp desc


select pt.PatientId, pt.Sex, Year(pt.DateOfBirth) as DOY, ps.PlanSetupId, ps.StatusDate
from 
	PlanSetup ps, 
	Course cs,
	Patient pt
where ps.CourseSer = cs.CourseSer
	and cs.PatientSer = pt.PatientSer
	and ps.Status = 'TreatApproval'
	and ps.HstryDateTime between '01/01/2000 00:01' and '12/31/2024 23:59'
	and LOWER(ps.PlanSetupId) like 'bladder%'
	order by ps.HstryDateTime desc


select top 10 * from VARIAN.dbo.Structure s
where LOWER(s.StructureId) like 'bladder%'
order by s.HstryDateTime desc

select '_'+pt.PatientId as 'MRN', pt.LastName,  img.ImageId, img.CreationDate, img.CreationUserName, img.ImageSizeX, img.ImageSizeY, img.ImageSizeZ, img.ImageResX, img.ImageResY, img.ImageResZ, sset.StructureSetId, s.StructureId, s.StatusDate, s.StatusUserName
from Structure s, StructureSet sset, Image img, Patient pt
where s.StructureSetSer = sset.StructureSetSer
and sset.ImageSer = img.ImageSer
and LOWER(s.StructureId) like 'bladder%'
and LOWER(img.ImageId) like '%cbct%'
and img.PatientSer = pt.PatientSer
and s.StatusUserName = 'uhmc\jkim20'
order by s.HstryDateTime desc



select top 20 * from VARIAN.dbo.Image img
where LOWER(img.ImageId) like '%cbct%'
and img.ImageType = 'Image'




select top 10 * from Patient pt

select pt.PatientId, ps.PlanSetupId, rtp.NoFractions, rtp.PrescribedDose, ps.HstryDateTime
from 
	PlanSetup ps, 
	RTPlan rtp,
	Course cs,
	Patient pt,
	PatientDoctor pd,
	Doctor d
where ps.CourseSer = cs.CourseSer
	and ps.PlanSetupSer = rtp.PlanSetupSer
	and cs.PatientSer = pt.PatientSer
	and pt.PatientSer = pd.PatientSer
	and pd.ResourceSer = d.ResourceSer
	and ps.Status = 'TreatApproval'
	and ps.HstryDateTime between '01/01/2000 00:01' and '12/31/2024 23:59'
	and d.LastName = 'Slade'
	and LOWER(ps.PlanSetupId) like 'bladder%'
order by ps.HstryDateTime desc

select top 10 * from Prescription
select top 10 * from ExternalBeam
select top 10 * from RTPlan
select top 10 * from vv_RTPlan
select top 10 * from vv_PlanSetup
select top 10 * from vv_Radiation
select top 10 * from vv_Radiation r, PlanSetup ps
where r.PlanSetupSer = ps.PlanSetupSer
and r.PatientId = '30732091'









SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE COLUMN_NAME = 'PlanSetupSer';

SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE COLUMN_NAME = 'RTPlanSer';




select d.LastName, d.FirstName, d.DoctorId
from PatientDoctor pd, Patient p, Doctor d
where pd.PatientSer = p.PatientSer and pd.ResourceSer = d.ResourceSer
and pd.OncologistFlag = 1
and pd.PrimaryFlag = 1
and p.PatientId = '00015034'


select d.LastName, d.FirstName, d.DoctorId, d.*
from Doctor d

