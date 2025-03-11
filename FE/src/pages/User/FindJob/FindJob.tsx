import React, { useEffect, useState } from "react";
import { IoSearch } from "react-icons/io5";
import Header from "../../../components/Header/Header";
import Job from "../../../components/Cards/Job/Job";
import FPTLogo from "../../../assets/logo-fpt.jpg";
import "./FindJob.css";
import { ApiGateway } from "../../../services/api/ApiService";

interface jobcard {
	id: string,
	companyLogo: string,
	companyName: string,
	title: string,
	lastPosted: number,
	workLocation: string
}

const FindJob: React.FC = () => {

	const [jobPosts, setJobPosts] = useState<jobcard[]>([]);

	useEffect(() => {
		fetchingPosts()
	}, [])

	const fetchingPosts = async () => {
		const data = await ApiGateway.ListPost<jobcard[]>(8, 1)
		setJobPosts(data);
	}

	return (
		<div id="find_job">
			<Header />
			<div className="job-board-container">
				<div className="job-list-section">
					<p style={{ fontSize: '1.5em', fontWeight: '700' }}>Job Board</p>
					<div className="search-bar">
						<input type="text" placeholder="Company, Skills, Tag" />
						<IoSearch size={20} className="search-icon" />
					</div>
					<p style={{ fontSize: "1.2em", fontWeight: '600' }}>Recent Post</p>
					<div className="job-list">
						{jobPosts.map((job, index) => (
							<a href={`/job-detail/${job.id}`}>
								<Job
									key={index}
									img={job.companyLogo}
									company={job.companyName}
									title={job.title}
									location={job.workLocation}
									timePosted={job.lastPosted}
									mini={false}
								/>
							</a>
						))}
					</div>
				</div>

				<div className="filter-section">
					<h3>Specialties</h3>
					{[
						"Animation",
						"Brand",
						"Illustration",
						"Mobile Design",
						"Print",
						"Product Design",
						"Typography",
						"Web Design",
					].map((specialty) => (
						<div key={specialty} className="filter-option">
							<input type="checkbox" />
							<label>{specialty}</label>
						</div>
					))}
					<h3>Location</h3>
					<input type="text" placeholder="Enter Location" className="location-input" />
					<h3>Job Type</h3>
					<div className="filter-option">
						<input type="checkbox" /> <label>Full-time</label>
					</div>
					<div className="filter-option">
						<input type="checkbox" /> <label>Freelance/Contract</label>
					</div>
					<button className="filter-button">Filter</button>
				</div>
			</div>
		</div>
	);
};

export default FindJob;
