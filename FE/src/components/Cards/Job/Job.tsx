import React from 'react'
import './Job.css'

interface Job{
    img: string;
    company: string;
    title: string;
    spectialty: string[];
    location: string;
    jType: string;
    timePosted: string;
}

const Job : React.FC<Job> = ({img, company, title, location, spectialty, jType, timePosted}) => {
  return (
    <div id="job_card">
        <div className="job-info">
            <img
            src={img}
            alt="Company Logo"
            className="company-logo"
            />
            <div>
            <h4>{company}</h4>
            <p>{title}</p>
            </div>
        </div>
        <div className="job-meta">
            <p>{timePosted}</p>
            <p>{location}</p>
        </div>
    </div>
  )
}

export default Job