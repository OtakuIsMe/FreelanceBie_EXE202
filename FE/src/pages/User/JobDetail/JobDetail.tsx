import React, { useEffect, useState } from "react";
import FPTLogo from "../../../assets/logo-fpt.jpg";
import "./JobDetail.css";
import Job from "../../../components/Cards/Job/Job";
import JobInfo, { FileProps } from "../../../components/JobInfo/JobInfo";
import { useParams } from "react-router-dom";
import { ApiGateway } from "../../../services/api/ApiService";
interface JobPostProps {
	title: string;
	description: string;
	workType: number;
	workLocation: string;
	companyName: string;
	companyLogo: string;
	specialty: string;
	experience: number;
	employmentType: number;
	files: FileProps[];
}
const JobDetail: React.FC = () => {
	const { id } = useParams();
	const [jobPosts, setJobPost] = useState<JobPostProps | null>(null)

	const sameJobs = Array(4).fill({
		company: "FPT Software",
		title: "Senior UX/UI Designer",
		location: "Ho Chi Minh",
		specialty: ["Web Design", "Mobile Design"],
		jType: "Full-time",
		timePosted: "1 hour ago",
	});
	useEffect(() => {
		fetchingJobDetail();
	}, [id])

	const fetchingJobDetail = async () => {
		console.log(id)
		if (!id) {
			console.error("ID is undefined, cannot fetch job details.");
			return;
		}

		const data = await ApiGateway.JobDetail<JobPostProps>(id);
		setJobPost(data);
	};

	return (
		<div id="job_detail">
			<div className="job-detail-container">
				<div className="job-left-column">
					{jobPosts && (
						<JobInfo
							title={jobPosts.title}
							description={jobPosts.description}
							workType={jobPosts.workType}
							workLocation={jobPosts.workLocation}
							companyName={jobPosts.companyName}
							companyLogo={jobPosts.companyLogo}
							specialty={jobPosts.specialty}
							experience={jobPosts.experience}
							employmentType={jobPosts.employmentType}
							files={jobPosts.files}
						/>
					)}
				</div>

				<div className="job-right-column">
					<div className="similar-jobs">
						<p className="tittle">Similar jobs</p>
						{sameJobs.map((job, index) => (
							<Job
								key={index}
								img={FPTLogo}
								company={job.company}
								title={job.title}
								location={job.location}
								spectialty={job.specialty}
								jType={job.jType}
								timePosted={job.timePosted}
								mini={true}
							/>
						))}
					</div>
				</div>
			</div>
		</div>
	);
};

export default JobDetail;
