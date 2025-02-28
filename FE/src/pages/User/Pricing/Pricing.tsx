import React from 'react'
import Header from '../../../components/Header/Header'
import Footer from '../../../components/Footer/Footer'
import { useNavigate } from 'react-router-dom'
import './Pricing.css'

const Pricing :React.FC = () => {

    const navigate = useNavigate();
    
    return (
        <div id="pricing">
            <Header/>
            <div className="pricing-container">
                <h3 className="pricing-header">Friendly Pricing</h3>
                <p className="pricing-subheader">
                    A new and better way to acquire, engage and support customers
                </p>
                <div className="pricing-plans">
                    <div className="plan free">
                        <h4>FREE</h4>
                        <p>Essentials tools for individuals and talents</p>
                        <h2>$0</h2>
                        <hr />
                        <div className="benefit">
                            <ul>
                                <li>Dashboard Access</li>
                                <li>Customer Support</li>
                                <li className="disabled">Unlimited Campaigns</li>
                                <li className="disabled">Unlimited Influencers</li>
                                <li className="disabled">Fraud Prevention</li>
                            </ul>
                            <button className="choose-button free" onClick={() => navigate("/inspiration")}>Start</button>
                        </div>
                    </div>

                    <div className="plan enterprise">
                        <h4>ENTERPRISE</h4>
                        <p>Essentials tools for individuals and talents</p>
                        <h2>$79 <span>/Per month</span></h2>
                        <hr />
                        <div className="benefit">
                            <ul>
                                <li>Dashboard Access</li>
                                <li>Customer Support</li>
                                <li>Unlimited Campaigns</li>
                                <li>Unlimited Influencers</li>
                                <li>Fraud Prevention</li>
                            </ul>
                            <button className="choose-button enterprise" onClick={() => navigate("/")}>Start</button>
                        </div>
                    </div>
                </div>
            </div>
            <Footer/>
        </div>
    )
}

export default Pricing