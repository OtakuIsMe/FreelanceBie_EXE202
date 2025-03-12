import React from 'react'
import Header from '../../../components/Header/Header'
import Footer from '../../../components/Footer/Footer'
import crowd from '../../../assets/crowd.avif'
import conversion from '../../../assets/conversion.avif'
import transact from '../../../assets/transact.avif'
import './Pricing.css'

const Pricing :React.FC = () => {

    return (
        <div id="pricing">
            <Header/>
			<div className="content-background">
				<div className="content-container">
					<div className="membership-container">
						<div className="title-container">
							<p className="logo">FREELANCEBIE PRO</p>
							<p className="title">Get more leads, pay no fees.</p>
							<p className="des">Unbeatable ROI for professional designers growing their business.</p>
						</div>
						<div className="membership">
							<div className="price-container">
								<p className="price">$8</p>
								<p className="quanty">per month</p>
							</div>
							<ul>
								<li>
								<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M18.577 6.183a1 1 0 0 1 .24 1.394l-5.666 8.02c-.36.508-.665.94-.94 1.269c-.287.34-.61.658-1.038.86a2.83 2.83 0 0 1-2.03.153c-.456-.137-.82-.406-1.149-.702c-.315-.285-.672-.668-1.09-1.116l-1.635-1.753a1 1 0 1 1 1.462-1.364l1.606 1.722c.455.487.754.806.998 1.027c.24.216.344.259.385.271c.196.06.405.045.598-.046c.046-.022.149-.085.36-.338c.216-.257.473-.62.863-1.171l5.642-7.986a1 1 0 0 1 1.394-.24"/></svg>
									No fee transactions (annual plans only)</li>
								<li>
								<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M18.577 6.183a1 1 0 0 1 .24 1.394l-5.666 8.02c-.36.508-.665.94-.94 1.269c-.287.34-.61.658-1.038.86a2.83 2.83 0 0 1-2.03.153c-.456-.137-.82-.406-1.149-.702c-.315-.285-.672-.668-1.09-1.116l-1.635-1.753a1 1 0 1 1 1.462-1.364l1.606 1.722c.455.487.754.806.998 1.027c.24.216.344.259.385.271c.196.06.405.045.598-.046c.046-.022.149-.085.36-.338c.216-.257.473-.62.863-1.171l5.642-7.986a1 1 0 0 1 1.394-.24"/></svg>
									Ranking boost in feeds</li>
								<li>
								<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M18.577 6.183a1 1 0 0 1 .24 1.394l-5.666 8.02c-.36.508-.665.94-.94 1.269c-.287.34-.61.658-1.038.86a2.83 2.83 0 0 1-2.03.153c-.456-.137-.82-.406-1.149-.702c-.315-.285-.672-.668-1.09-1.116l-1.635-1.753a1 1 0 1 1 1.462-1.364l1.606 1.722c.455.487.754.806.998 1.027c.24.216.344.259.385.271c.196.06.405.045.598-.046c.046-.022.149-.085.36-.338c.216-.257.473-.62.863-1.171l5.642-7.986a1 1 0 0 1 1.394-.24"/></svg>
									Advanced profiles</li>
								<li>
								<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M18.577 6.183a1 1 0 0 1 .24 1.394l-5.666 8.02c-.36.508-.665.94-.94 1.269c-.287.34-.61.658-1.038.86a2.83 2.83 0 0 1-2.03.153c-.456-.137-.82-.406-1.149-.702c-.315-.285-.672-.668-1.09-1.116l-1.635-1.753a1 1 0 1 1 1.462-1.364l1.606 1.722c.455.487.754.806.998 1.027c.24.216.344.259.385.271c.196.06.405.045.598-.046c.046-.022.149-.085.36-.338c.216-.257.473-.62.863-1.171l5.642-7.986a1 1 0 0 1 1.394-.24"/></svg>
									Ad free browsing</li>
							</ul>
							<button className="join">Join Pro</button>
						</div>
					</div>
					<div className="framer-container">
						<div className="card">
							<img src={crowd} alt="" />
							<p className="title">Stand out from the crowd</p>
							<p className="des">Rank higher in feeds to generate more leads.</p>
						</div>
						<div className="card">
							<img src={conversion} alt="" />
							<p className="title">Improve conversion</p>
							<p className="des">Access advanced features that will help convert more leads.</p>
						</div>
						<div className="card">
							<img src={transact} alt="" />
							<p className="title">Transact for free</p>
							<p className="des">Pay no fees on transactions completed through Dribbble.</p>
						</div>
					</div>
				</div>
			</div>
            <Footer/>
        </div>
    )
}

export default Pricing
