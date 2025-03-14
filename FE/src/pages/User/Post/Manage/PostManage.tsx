import { useEffect, useState } from 'react';
import * as React from 'react';
import Header from '../../../../components/Header/Header';
import './PostManage.css';
import Stack from '@mui/material/Stack';
import LinearProgress, { linearProgressClasses } from '@mui/material/LinearProgress';
import { styled } from '@mui/material/styles';


interface jobCard {
	title: string,
	status: boolean,
	workType: number,
	workLocation: string,
	employmentType: number,
	createAt: Date,
	closeAt: Date,
	numberApplied: number,
	numberHired: number,
	id: string,
	Specialty: string
}

interface DropdownBarProp {
	options: string[],
	allTitle: string,
	svgIcon: React.ReactNode;
}

const PostManage: React.FC = () => {
	const [jobs, setJobs] = useState<jobCard[]>([])

	useEffect(() => {
		setJobs([
			{
				title: "Frontend Developer",
				status: true,
				workType: 0, // Remote
				workLocation: "Ho Chi Minh",
				employmentType: 1, // Part-Time
				createAt: new Date("2025-01-01"),
				closeAt: new Date("2025-04-01"),
				numberApplied: 50,
				numberHired: 5,
				id: "job1",
				Specialty: "React, TypeScript",
			},
			{
				title: "Backend Developer",
				status: true,
				workType: 1, // Onsite
				workLocation: "Hanoi",
				employmentType: 0, // Full-Time
				createAt: new Date("2025-02-15"),
				closeAt: new Date("2025-05-15"),
				numberApplied: 80,
				numberHired: 10,
				id: "job2",
				Specialty: "Node.js, .NET",
			},
			{
				title: "UX/UI Designer",
				status: false,
				workType: 2, // Hybrid
				workLocation: "Da Nang",
				employmentType: 2, // Contract
				createAt: new Date("2025-03-10"),
				closeAt: new Date("2025-06-10"),
				numberApplied: 30,
				numberHired: 3,
				id: "job3",
				Specialty: "Figma, Adobe XD",
			},
			{
				title: "Frontend Developer",
				status: true,
				workType: 0, // Remote
				workLocation: "Ho Chi Minh",
				employmentType: 1, // Part-Time
				createAt: new Date("2025-01-01"),
				closeAt: new Date("2025-04-01"),
				numberApplied: 50,
				numberHired: 5,
				id: "job1",
				Specialty: "React, TypeScript",
			},
			{
				title: "Backend Developer",
				status: true,
				workType: 1, // Onsite
				workLocation: "Hanoi",
				employmentType: 0, // Full-Time
				createAt: new Date("2025-02-15"),
				closeAt: new Date("2025-05-15"),
				numberApplied: 80,
				numberHired: 10,
				id: "job2",
				Specialty: "Node.js, .NET",
			},
			{
				title: "UX/UI Designer",
				status: false,
				workType: 2, // Hybrid
				workLocation: "Da Nang",
				employmentType: 2, // Contract
				createAt: new Date("2025-03-10"),
				closeAt: new Date("2025-06-10"),
				numberApplied: 30,
				numberHired: 3,
				id: "job3",
				Specialty: "Figma, Adobe XD",
			},
		]);
	}, []);

	const employmentTypeMap: Record<number, string> = {
		0: "Full-Time",
		1: "Part-Time",
		2: "Contract",
	};

	const workTypeMap: Record<number, string> = {
		0: "Remote",
		1: "Onsite",
		2: "Hybrid",
	};

	const getEmploymentType = (type: number): string => {
		return employmentTypeMap[type] || "Unknown";
	};

	const getWorkType = (type: number): string => {
		return workTypeMap[type] || "Unknown";
	};

	const formatDate = (date: Date): string => {
		return date.toLocaleDateString("en-GB", {
			day: "2-digit",
			month: "short",
		});
	};

	const getDaysToGo = (closeAt: Date): string => {
		const today = new Date();
		const diffTime = closeAt.getTime() - today.getTime();
		const daysRemaining = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

		return daysRemaining > 0 ? `${daysRemaining} Days to go` : "Closed";
	};

	const getProgress = (createAt: Date, closeAt: Date): number => {
		const totalDuration = closeAt.getTime() - createAt.getTime();
		const elapsedTime = new Date().getTime() - createAt.getTime();

		const progress = (elapsedTime / totalDuration) * 100;

		return Math.min(Math.max(progress, 0), 100);
	};



	const DropdownBar: React.FC<DropdownBarProp> = ({ svgIcon, options, allTitle }) => {
		const [selectOptions, setSelectOptions] = useState<
			{
				title: string,
				isSelected: boolean
			}[]
		>(options.map((option) => ({ title: option, isSelected: true })))

		const [isOpenDropdown, setIsOpenDropdown] = useState<boolean>(false)

		const handleDropdown = () => {
			setIsOpenDropdown(prev => !prev)
		}

		const handleChange = (index: number) => {
			setSelectOptions((prev) =>
				prev.map((option, i) =>
					i === index ? { ...option, isSelected: !option.isSelected } : option
				)
			);
		};

		return (
			<div className="dropdown-bar">
				<div className="icon">{svgIcon}</div>
				<p className='title'>{allTitle} </p>
				<button className="btn-dropdown" onClick={handleDropdown}>
					<svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 48 48"><path fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="4" d="M36 18L24 30L12 18" /></svg>
				</button>
				{isOpenDropdown && (
					<div className="options">
						{selectOptions.map((option, index) => {
							return (
								<div className={`option ${index + 1 === selectOptions.length ? "last" : ""}`} key={index}>
									<input type="checkbox" name="" id=""
										checked={option.isSelected}
										onChange={() => handleChange(index)} />
									<p className="name">{option.title}</p>
								</div>
							)
						})}
					</div>
				)}
			</div>
		)
	}

	const BorderLinearProgress = styled(LinearProgress)<{ status?: boolean }>(({ theme, status }) => ({
		height: 5,
		borderRadius: 5,
		[`&.${linearProgressClasses.colorPrimary}`]: {
			backgroundColor: theme.palette.grey[200],
			...theme.applyStyles('dark', {
				backgroundColor: theme.palette.grey[800],
			}),
		},
		[`& .${linearProgressClasses.bar}`]: {
			borderRadius: 5,
			backgroundColor: status === true ? '#9bbe7f' : '#b4431e',
			...theme.applyStyles('dark', {
				backgroundColor: '#308fe8',
			}),
		},
	}));

	return (
		<div id="post-manage">
			<Header />
			<div className="manage-container">
				<div className="title-container">
					<p className="title">Post Opening</p>
				</div>
				<div className="status-counting">
					<div className="status open">
						<svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 15 15"><path fill="currentColor" d="M9.875 7.5a2.375 2.375 0 1 1-4.75 0a2.375 2.375 0 0 1 4.75 0" /></svg>
						Open
					</div>
					<div className="status close">
						<svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 15 15"><path fill="currentColor" d="M9.875 7.5a2.375 2.375 0 1 1-4.75 0a2.375 2.375 0 0 1 4.75 0" /></svg>
						Close
					</div>
				</div>
				<div className="action-bar">
					<div className="search-bar">
						<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><g fill="none" fill-rule="evenodd"><path d="m12.593 23.258l-.011.002l-.071.035l-.02.004l-.014-.004l-.071-.035q-.016-.005-.024.005l-.004.01l-.017.428l.005.02l.01.013l.104.074l.015.004l.012-.004l.104-.074l.012-.016l.004-.017l-.017-.427q-.004-.016-.017-.018m.265-.113l-.013.002l-.185.093l-.01.01l-.003.011l.018.43l.005.012l.008.007l.201.093q.019.005.029-.008l.004-.014l-.034-.614q-.005-.018-.02-.022m-.715.002a.02.02 0 0 0-.027.006l-.006.014l-.034.614q.001.018.017.024l.015-.002l.201-.093l.01-.008l.004-.011l.017-.43l-.003-.012l-.01-.01z" /><path fill="currentColor" d="M10.5 2a8.5 8.5 0 1 0 5.262 15.176l3.652 3.652a1 1 0 0 0 1.414-1.414l-3.652-3.652A8.5 8.5 0 0 0 10.5 2M4 10.5a6.5 6.5 0 1 1 13 0a6.5 6.5 0 0 1-13 0" /></g></svg>
						<input type="text" name="" id="" placeholder='Search Job' />
					</div>
					<div className="categories selection-bar">
						<DropdownBar allTitle='All Categories'
							options={["Designer", "Mobile", "UI"]}
							svgIcon={
								<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M7.425 9.475L11.15 3.4q.15-.25.375-.363T12 2.925t.475.113t.375.362l3.725 6.075q.15.25.15.525t-.125.5t-.35.363t-.525.137h-7.45q-.3 0-.525-.137T7.4 10.5t-.125-.5t.15-.525M17.5 22q-1.875 0-3.187-1.312T13 17.5t1.313-3.187T17.5 13t3.188 1.313T22 17.5t-1.312 3.188T17.5 22M3 20.5v-6q0-.425.288-.712T4 13.5h6q.425 0 .713.288T11 14.5v6q0 .425-.288.713T10 21.5H4q-.425 0-.712-.288T3 20.5" /></svg>
							} />
					</div>
					<div className="statuses selection-bar">
						<DropdownBar allTitle='All Status'
							options={["Open", "Close"]}
							svgIcon={
								<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="none" stroke="currentColor" stroke-width="2" d="M1 13h4l2.5-9l5 16.5L17 9l2 4h4" /></svg>
							} />
					</div>
				</div>
				<div className="jobs-container">
					{jobs.map((job, index) => {
						return (
							<div className="job-card" key={index}>
								<div className="status-edit-btn">
									<div className="status-type">
										<div className="status" style={job.status ? { backgroundColor: "#9bbe7f" } : { backgroundColor: "#b4431e" }}>
											<svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 15 15"><path fill="currentColor" d="M9.875 7.5a2.375 2.375 0 1 1-4.75 0a2.375 2.375 0 0 1 4.75 0" /></svg>
											<span>{job.status ? "Open" : "Close"}</span>
											<svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 48 48"><path fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="4" d="M36 18L24 30L12 18" /></svg>
										</div>
										<div className="type">
											{job.Specialty}
										</div>
									</div>
									<div className="edit">
										<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><g fill="none"><path d="m12.593 23.258l-.011.002l-.071.035l-.02.004l-.014-.004l-.071-.035q-.016-.005-.024.005l-.004.01l-.017.428l.005.02l.01.013l.104.074l.015.004l.012-.004l.104-.074l.012-.016l.004-.017l-.017-.427q-.004-.016-.017-.018m.265-.113l-.013.002l-.185.093l-.01.01l-.003.011l.018.43l.005.012l.008.007l.201.093q.019.005.029-.008l.004-.014l-.034-.614q-.005-.018-.02-.022m-.715.002a.02.02 0 0 0-.027.006l-.006.014l-.034.614q.001.018.017.024l.015-.002l.201-.093l.01-.008l.004-.011l.017-.43l-.003-.012l-.01-.01z" /><path fill="currentColor" d="M6 10.5a1.5 1.5 0 1 1 0 3a1.5 1.5 0 0 1 0-3m6 0a1.5 1.5 0 1 1 0 3a1.5 1.5 0 0 1 0-3m6 0a1.5 1.5 0 1 1 0 3a1.5 1.5 0 0 1 0-3" /></g></svg>
									</div>
								</div>
								<div className="title-job-container">
									<p className="title">
										{job.title}
									</p>
								</div>
								<div className="tags">
									<div className="tag">
										<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 1024 1024"><path fill="currentColor" d="M536.1 273H488c-4.4 0-8 3.6-8 8v275.3c0 2.6 1.2 5 3.3 6.5l165.3 120.7c3.6 2.6 8.6 1.9 11.2-1.7l28.6-39c2.7-3.7 1.9-8.7-1.7-11.2L544.1 528.5V281c0-4.4-3.6-8-8-8m219.8 75.2l156.8 38.3c5 1.2 9.9-2.6 9.9-7.7l.8-161.5c0-6.7-7.7-10.5-12.9-6.3L752.9 334.1a8 8 0 0 0 3 14.1m167.7 301.1l-56.7-19.5a8 8 0 0 0-10.1 4.8c-1.9 5.1-3.9 10.1-6 15.1c-17.8 42.1-43.3 80-75.9 112.5a353 353 0 0 1-112.5 75.9a352.2 352.2 0 0 1-137.7 27.8c-47.8 0-94.1-9.3-137.7-27.8a353 353 0 0 1-112.5-75.9c-32.5-32.5-58-70.4-75.9-112.5A353.4 353.4 0 0 1 171 512c0-47.8 9.3-94.2 27.8-137.8c17.8-42.1 43.3-80 75.9-112.5a353 353 0 0 1 112.5-75.9C430.6 167.3 477 158 524.8 158s94.1 9.3 137.7 27.8A353 353 0 0 1 775 261.7c10.2 10.3 19.8 21 28.6 32.3l59.8-46.8C784.7 146.6 662.2 81.9 524.6 82C285 82.1 92.6 276.7 95 516.4C97.4 751.9 288.9 942 524.8 942c185.5 0 343.5-117.6 403.7-282.3c1.5-4.2-.7-8.9-4.9-10.4" /></svg>
										{getEmploymentType(job.employmentType)}
									</div>
									<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 256 256"><path fill="currentColor" d="M144 128a16 16 0 1 1-16-16a16 16 0 0 1 16 16" /></svg>
									<div className="tag">
										<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><g fill="none" stroke="currentColor" stroke-linejoin="round" stroke-width="2"><path d="M13 9a1 1 0 1 1-2 0a1 1 0 0 1 2 0Z" /><path d="M17.5 9.5c0 3.038-2 6.5-5.5 10.5c-3.5-4-5.5-7.462-5.5-10.5a5.5 5.5 0 1 1 11 0Z" /></g></svg>
										{job.workLocation}
									</div>
									<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 256 256"><path fill="currentColor" d="M144 128a16 16 0 1 1-16-16a16 16 0 0 1 16 16" /></svg>
									<div className="tag">
										<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M19 6.5h-3v-1a3 3 0 0 0-3-3h-2a3 3 0 0 0-3 3v1H5a3 3 0 0 0-3 3v9a3 3 0 0 0 3 3h14a3 3 0 0 0 3-3v-9a3 3 0 0 0-3-3m-9-1a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1v1h-4Zm10 13a1 1 0 0 1-1 1H5a1 1 0 0 1-1-1v-5.05h3v1.05a1 1 0 0 0 2 0v-1.05h6v1.05a1 1 0 0 0 2 0v-1.05h3Zm0-7H4v-2a1 1 0 0 1 1-1h14a1 1 0 0 1 1 1Z" /></svg>
										{getWorkType(job.workType)}
									</div>
								</div>
								<div className="counting-container">
									<div className="applied box">
										<p className="count">{job.numberApplied}</p>
										<p className="title">Employee Applied</p>
									</div>
									<div className="hiring box">
										<p className="count">{job.numberApplied}</p>
										<p className="title">Employee Hired</p>
									</div>
								</div>
								<div className="plan-container">
									<div className="time-container">
										<div className="duration-time">
											<svg className='date' xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M16.5 5V3m-9 2V3M3.25 8h17.5M3 10.044c0-2.115 0-3.173.436-3.981a3.9 3.9 0 0 1 1.748-1.651C6.04 4 7.16 4 9.4 4h5.2c2.24 0 3.36 0 4.216.412c.753.362 1.364.94 1.748 1.65c.436.81.436 1.868.436 3.983v4.912c0 2.115 0 3.173-.436 3.981a3.9 3.9 0 0 1-1.748 1.651C17.96 21 16.84 21 14.6 21H9.4c-2.24 0-3.36 0-4.216-.412a3.9 3.9 0 0 1-1.748-1.65C3 18.128 3 17.07 3 14.955z" /></svg>
											<span>
												Posted at {formatDate(job.createAt)}
											</span>
											<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 16 16"><path fill="currentColor" d="M8 9.5a1.5 1.5 0 1 0 0-3a1.5 1.5 0 0 0 0 3" /></svg>
											<span>
												Close at {formatDate(job.closeAt)}
											</span>
										</div>
										<div className="time-left">
											<span>{getDaysToGo(job.closeAt)}</span>
										</div>
									</div>
									<div className="progress-bar-container">
										<Stack spacing={1} sx={{ flexGrow: 1 }}>
											<BorderLinearProgress variant="determinate" value={getProgress(job.createAt, job.closeAt)} status={job.status} />
										</Stack>
									</div>
								</div>
								<div className="detail-container">
									<p className="detail">
										<span>View Details</span>
										<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 20 20"><path fill="currentColor" d="M7 1L5.6 2.5L13 10l-7.4 7.5L7 19l9-9z" /></svg>
									</p>
								</div>
							</div>
						)
					})}
				</div>
			</div>
		</div>
	)
}

export default PostManage;
