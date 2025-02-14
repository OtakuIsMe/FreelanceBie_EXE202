import React from "react";
import { IoSearch } from "react-icons/io5";
import Header from "../../../components/Header/Header";
import Job from "../../../components/Cards/Job/Job";
import FPTLogo from "../../../assets/logo-fpt.jpg";
import "./FindJob.css";

const FindJob: React.FC = () => {
  // Mock data for job posts
  const jobPosts = Array(6).fill({
    company: "FPT Software",
    title: "Senior UX/UI Designer",
    location: "Ho Chi Minh",
    specialty: ["Web Design", "Mobile Design"],
    jType: "Full-time",
    timePosted: "1 hour ago",
  });

  return (
    <div id="find_job">
      <Header/>
      <div className="job-board-container">
        <div className="job-list-section">
          <p style={{fontSize: '1.5em', fontWeight: '700'}}>Job Board</p>
          <div className="search-bar">
            <input type="text" placeholder="Company, Skills, Tag" />
            <IoSearch size={20} className="search-icon" />
          </div>
          <p  style={{fontSize: "1.2em", fontWeight: '600'}}>Recent Post</p>
          <div className="job-list">
            {jobPosts.map((job, index) => (
              <Job
                key={index}
                img={FPTLogo}
                company={job.company}
                title={job.title}
                location={job.location}
                spectialty={job.specialty}
                jType={job.jType}
                timePosted={job.timePosted}
                mini={false}
              />
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
