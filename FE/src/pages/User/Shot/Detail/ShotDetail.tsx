import { useEffect, useState } from 'react';
import './ShotDetail.css'
import { useParams } from "react-router-dom";
import { ApiGateway } from '../../../../services/api/ApiService';
import { useNavigate } from 'react-router-dom';

interface designProps {
	title: string;
	html: string;
	owner: {
		image: string;
		name: string;
		status: string;
		slogan: string;
	};
	user?: {
		isLiked: boolean;
		isSaved: boolean;
	};
}

interface otherDesignProps {
	id: string;
	title: string;
	imageVideos: [
		{
			url: string
		}
	]
}

const ShotDetail = () => {
	const { id } = useParams();
	const [design, setDesign] = useState<designProps | null>(null)
	const [otherDesigns, setOtherDesgin] = useState<otherDesignProps[] | null>(null)
	const navigate = useNavigate()

	useEffect(() => {
		const fetchData = async () => {
			try {
				if (id) {
					const data = await ApiGateway.ShotDetail<designProps>(id);
					setDesign(data)
					const data2 = await ApiGateway.ShotOther<otherDesignProps[]>(id);
					setOtherDesgin(data2)
				}
			} catch (error) {
				console.error("Error fetching shot details:", error);
			}
		};

		fetchData();
	}, [id]);

	return (
		<div id="Shot-Detail">
			<div className="shot-container">
				<p className="title-shot">{design?.title}</p>
				<div className="user-contact">
					<div className="user-status">
						<img src={design?.owner.image} alt="" />
						<div className="name-status">
							<p className="name">{design?.owner.name}</p>
							<p className={`status ${design?.owner?.status ? "active" : ""}`}>{design?.owner.status ? "Available for work" : "Unailable for work"}</p>
						</div>
					</div>
					<div className="contact-btns">
						<div className="like button">
							<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="m8.962 18.91l.464-.588zM12 5.5l-.54.52a.75.75 0 0 0 1.08 0zm3.038 13.41l.465.59zm-5.612-.588C7.91 17.127 6.253 15.96 4.938 14.48C3.65 13.028 2.75 11.335 2.75 9.137h-1.5c0 2.666 1.11 4.7 2.567 6.339c1.43 1.61 3.254 2.9 4.68 4.024zM2.75 9.137c0-2.15 1.215-3.954 2.874-4.713c1.612-.737 3.778-.541 5.836 1.597l1.08-1.04C10.1 2.444 7.264 2.025 5 3.06C2.786 4.073 1.25 6.425 1.25 9.137zM8.497 19.5c.513.404 1.063.834 1.62 1.16s1.193.59 1.883.59v-1.5c-.31 0-.674-.12-1.126-.385c-.453-.264-.922-.628-1.448-1.043zm7.006 0c1.426-1.125 3.25-2.413 4.68-4.024c1.457-1.64 2.567-3.673 2.567-6.339h-1.5c0 2.198-.9 3.891-2.188 5.343c-1.315 1.48-2.972 2.647-4.488 3.842zM22.75 9.137c0-2.712-1.535-5.064-3.75-6.077c-2.264-1.035-5.098-.616-7.54 1.92l1.08 1.04c2.058-2.137 4.224-2.333 5.836-1.596c1.659.759 2.874 2.562 2.874 4.713zm-8.176 9.185c-.526.415-.995.779-1.448 1.043s-.816.385-1.126.385v1.5c.69 0 1.326-.265 1.883-.59c.558-.326 1.107-.756 1.62-1.16z" /></svg>
						</div>
						<div className="save button">
							<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 16 16"><path fill="currentColor" d="M2 2a2 2 0 0 1 2-2h8a2 2 0 0 1 2 2v13.5a.5.5 0 0 1-.777.416L8 13.101l-5.223 2.815A.5.5 0 0 1 2 15.5zm2-1a1 1 0 0 0-1 1v12.566l4.723-2.482a.5.5 0 0 1 .554 0L13 14.566V2a1 1 0 0 0-1-1z" /></svg>
						</div>
						<div className="in-touch button">
							Get in touch
						</div>
					</div>
				</div>
				<div className="shot-content" dangerouslySetInnerHTML={{ __html: design?.html || "" }}></div>
				<div className="user-endtouch">
					<div className="user-icon-container">
						<div className="user-icon">
							<img src={design?.owner.image} alt="" />
						</div>
					</div>
					<p className="username">{design?.owner.name}</p>
					<p className="slogan">{design?.owner.slogan}</p>
					<div className="in-touch button">
						Get in touch
					</div>
				</div>
				<div className="more-by-user">
					<div className="title-profile">
						<p className="title">More by {design?.owner.name}</p>
						<a href="" className='profile'>View profile</a>
					</div>
					<div className="other-design-container">
						{otherDesigns?.map((otherDesign, index) => {
							return (
								<div className="other-design" key={index} onClick={() => { navigate(`/shot/${otherDesign.id}`) }}>
									<div className='image-info'>
										<img src={otherDesign.imageVideos[0].url} alt="" className="background" />
										<div className="info-container">
											<div className="info-border">
												<div className="text">
													<p className="name">{otherDesign.title}</p>
												</div>
												<div className="like button">
													<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="m8.962 18.91l.464-.588zM12 5.5l-.54.52a.75.75 0 0 0 1.08 0zm3.038 13.41l.465.59zm-5.612-.588C7.91 17.127 6.253 15.96 4.938 14.48C3.65 13.028 2.75 11.335 2.75 9.137h-1.5c0 2.666 1.11 4.7 2.567 6.339c1.43 1.61 3.254 2.9 4.68 4.024zM2.75 9.137c0-2.15 1.215-3.954 2.874-4.713c1.612-.737 3.778-.541 5.836 1.597l1.08-1.04C10.1 2.444 7.264 2.025 5 3.06C2.786 4.073 1.25 6.425 1.25 9.137zM8.497 19.5c.513.404 1.063.834 1.62 1.16s1.193.59 1.883.59v-1.5c-.31 0-.674-.12-1.126-.385c-.453-.264-.922-.628-1.448-1.043zm7.006 0c1.426-1.125 3.25-2.413 4.68-4.024c1.457-1.64 2.567-3.673 2.567-6.339h-1.5c0 2.198-.9 3.891-2.188 5.343c-1.315 1.48-2.972 2.647-4.488 3.842zM22.75 9.137c0-2.712-1.535-5.064-3.75-6.077c-2.264-1.035-5.098-.616-7.54 1.92l1.08 1.04c2.058-2.137 4.224-2.333 5.836-1.596c1.659.759 2.874 2.562 2.874 4.713zm-8.176 9.185c-.526.415-.995.779-1.448 1.043s-.816.385-1.126.385v1.5c.69 0 1.326-.265 1.883-.59c.558-.326 1.107-.756 1.62-1.16z" /></svg>
												</div>
												<div className="save button">
													<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 16 16"><path fill="currentColor" d="M2 2a2 2 0 0 1 2-2h8a2 2 0 0 1 2 2v13.5a.5.5 0 0 1-.777.416L8 13.101l-5.223 2.815A.5.5 0 0 1 2 15.5zm2-1a1 1 0 0 0-1 1v12.566l4.723-2.482a.5.5 0 0 1 .554 0L13 14.566V2a1 1 0 0 0-1-1z" /></svg>
												</div>
											</div>
										</div>
									</div>
								</div>
							)
						})}
					</div>
				</div>
			</div>
		</div>
	)
}

export default ShotDetail;
