import { useEffect, useState } from 'react'
import './PostEmployee.css'
import Header from '../../../../components/Header/Header'
import { useSearchParams } from "react-router-dom";
import { ApiGateway } from '../../../../services/api/ApiService';

interface postInfo {
	title: string,
	createAt: string,
	closeAt: string,
	workLocation: string,
	numberApplied: number,
	numberHired: number,
	status: boolean
}
interface freelancerProp {
	image: string,
	username: string,
	place: string,
	price: number,
	status: number,
	email: string
}
const PostEmployee: React.FC = () => {
	const [searchParams] = useSearchParams();
	const jobId = searchParams.get("q");

	const [post, setPost] = useState<postInfo>({
		title: "",
		createAt: "",
		closeAt: "",
		workLocation: "",
		numberApplied: 0,
		numberHired: 0,
		status: false
	})

	const [freelancers, setFreelancers] = useState<freelancerProp[]>([])

	const [isOpenPopup, setIsOpenPopup] = useState<boolean>(false)

	useEffect(() => {
		if (jobId != null) {
			fetchFreelancer(jobId)
			fetchPostDetail(jobId)
		}
	}, [jobId])


	const fetchFreelancer = async (id: string) => {
		const data = await ApiGateway.ListApply<freelancerProp[]>(id)
		setFreelancers(data ?? []);
	}

	const fetchPostDetail = async (id: string) => {
		const data = await ApiGateway.PostEmployeeDetail<postInfo>(id)
		setPost(data);
	}

	const formatDate = (date: string): string => {
		const parsedDate = new Date(date);

		if (isNaN(parsedDate.getTime())) {
			return "Invalid Date";
		}

		return parsedDate.toLocaleDateString("en-US", {
			month: "short",
			day: "2-digit",
			year: "numeric",
		});
	};

	return (
		<div id="post-employee">
			<Header />
			<div className="post-info-container">
				<div className="btn-actions">
					<button className="back" onClick={() => { window.location.href = "/post/manage" }}>
						<svg xmlns="http://www.w3.org/2000/svg" width="12" height="24" viewBox="0 0 12 24"><path fill="currentColor" fill-rule="evenodd" d="m3.343 12l7.071 7.071L9 20.485l-7.778-7.778a1 1 0 0 1 0-1.414L9 3.515l1.414 1.414z" /></svg>
						Back to Posts
					</button>
					<button className="add-project">
						<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M5 13v-1h6V6h1v6h6v1h-6v6h-1v-6z" /></svg>
						Post
					</button>
				</div>
				<div className="title-status">
					<p className="title">{post.title}</p>
					<div className="status-edit">
						<div className={`status ${post.status ? "active" : ""}`}>
							<svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 15 15"><path fill="currentColor" d="M9.875 7.5a2.375 2.375 0 1 1-4.75 0a2.375 2.375 0 0 1 4.75 0" /></svg>
							{post.status ? "Open" : "Close"}
						</div>
						<div className="edit">
							<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><g fill="none"><path d="m12.593 23.258l-.011.002l-.071.035l-.02.004l-.014-.004l-.071-.035q-.016-.005-.024.005l-.004.01l-.017.428l.005.02l.01.013l.104.074l.015.004l.012-.004l.104-.074l.012-.016l.004-.017l-.017-.427q-.004-.016-.017-.018m.265-.113l-.013.002l-.185.093l-.01.01l-.003.011l.018.43l.005.012l.008.007l.201.093q.019.005.029-.008l.004-.014l-.034-.614q-.005-.018-.02-.022m-.715.002a.02.02 0 0 0-.027.006l-.006.014l-.034.614q.001.018.017.024l.015-.002l.201-.093l.01-.008l.004-.011l.017-.43l-.003-.012l-.01-.01z" /><path fill="currentColor" d="M12 17a2 2 0 1 1 0 4a2 2 0 0 1 0-4m0-7a2 2 0 1 1 0 4a2 2 0 0 1 0-4m0-7a2 2 0 1 1 0 4a2 2 0 0 1 0-4" /></g></svg>
						</div>
					</div>
				</div>
				<div className="detail-container">
					<div className="start tag">
						<p className="title">START DATE</p>
						<p className="info">{formatDate(post.createAt)}</p>
					</div>
					<div className="close tag">
						<p className="title">END DATE</p>
						<p className="info">{formatDate(post.closeAt)}</p>
					</div>
					<div className="location tag">
						<p className="title">LOCATION</p>
						<p className="info">{post.workLocation}</p>
					</div>
					<div className="applied tag">
						<p className="title">APPLIED</p>
						<p className="info">{post.numberApplied}</p>
					</div>
					<div className="hired tag">
						<p className="title">HIRED</p>
						<p className="info">{post.numberHired}</p>
					</div>
				</div>
				<div className="count-action">
					<p className="count">
						{freelancers.length} FREELANCERS FOUND
					</p>
					<div className="action-filter-container">
						<div className="search-bar">
							<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><g fill="none" fill-rule="evenodd"><path d="m12.593 23.258l-.011.002l-.071.035l-.02.004l-.014-.004l-.071-.035q-.016-.005-.024.005l-.004.01l-.017.428l.005.02l.01.013l.104.074l.015.004l.012-.004l.104-.074l.012-.016l.004-.017l-.017-.427q-.004-.016-.017-.018m.265-.113l-.013.002l-.185.093l-.01.01l-.003.011l.018.43l.005.012l.008.007l.201.093q.019.005.029-.008l.004-.014l-.034-.614q-.005-.018-.02-.022m-.715.002a.02.02 0 0 0-.027.006l-.006.014l-.034.614q.001.018.017.024l.015-.002l.201-.093l.01-.008l.004-.011l.017-.43l-.003-.012l-.01-.01z" /><path fill="currentColor" d="M10.5 2a8.5 8.5 0 1 0 5.262 15.176l3.652 3.652a1 1 0 0 0 1.414-1.414l-3.652-3.652A8.5 8.5 0 0 0 10.5 2M4 10.5a6.5 6.5 0 1 1 13 0a6.5 6.5 0 0 1-13 0" /></g></svg>
							<input type="text" name="" id="" placeholder='Search Job' />
						</div>
						<div className="status-hiring-filter" onClick={() => { setIsOpenPopup(prev => !prev) }}>
							<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><g fill="none" stroke="currentColor" stroke-linecap="round" stroke-width="1"><path d="M10 8h10M4 16h10" /><circle cx="7" cy="8" r="3" transform="rotate(90 7 8)" /><circle cx="17" cy="16" r="3" transform="rotate(90 17 16)" /></g></svg>
							<p className="title">Filters</p>
							{isOpenPopup && (
								<div className="dropdown" onClick={(e) => e.stopPropagation()}>
									<div className="filter apply">
										<input type="checkbox" name="" id="" />
										<p className="name">Apply</p>
									</div>
									<div className="filter deny">
										<input type="checkbox" name="" id="" />
										<p className="name">Deny</p>
									</div>
									<div className="filter not">
										<input type="checkbox" name="" id="" />
										<p className="name">Not Action</p>
									</div>
								</div>
							)}
						</div>
					</div>
				</div>
				<div className="freelancers">
					{freelancers.map((freelancer, index) => {
						return (
							<div className="freelancer" key={index}>
								<div className="image-name">
									<img src={freelancer.image} alt="" />
									<div className="name-email">
										<p className="name">{freelancer.username}</p>
										<p className="email">{freelancer.email}</p>
									</div>
								</div>
								<div className="location-price">
									<div className="location box">
										<p className="value">{freelancer.place}</p>
										<p className="title">LOCATION</p>
									</div>
									<div className="price box">
										<p className="value">{freelancer.price}</p>
										<p className="title">AVERAGE PRICE</p>
									</div>
								</div>
								<div className="action-btns">
									<div className={`accept btn ${freelancer.status === 0 ? "active" : ""}`}>
										{freelancer.status === 0 ? (
											<>
												<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 16 16"><path fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="m2.75 8.75l3.5 3.5l7-7.5" /></svg>
												Applied
											</>
										) : (
											<>
												<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M5 13v-1h6V6h1v6h6v1h-6v6h-1v-6z" /></svg>
												Apply
											</>
										)}
									</div>
									<div className={`deny btn ${freelancer.status === 1 ? "active" : ""}`}>
										{freelancer.status === 1 ? (
											<>
												<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 16 16"><path fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="m2.75 8.75l3.5 3.5l7-7.5" /></svg>
												Denied
											</>
										) : (
											<>
												<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M6.758 17.243L12.001 12m5.243-5.243L12 12m0 0L6.758 6.757M12.001 12l5.243 5.243" /></svg>
												Deny
											</>
										)}
									</div>
								</div>
								<div className="status-des">
									{freelancer.status === 0 && (
										<p className="des hired">
											Recently Hired - <span>Available</span>
										</p>
									)}
									{freelancer.status === 1 && (
										<p className="des denied">
											Recently Denied - <span>Unavailable</span>
										</p>
									)}
									{freelancer.status === 2 && (
										<p className="des no">
											There is no action
										</p>
									)}
								</div>
							</div>
						)
					})}
				</div>
			</div>
		</div >
	)
}

export default PostEmployee;
