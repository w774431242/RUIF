using System;

namespace UnityEngine
{
	// Token: 0x020001E2 RID: 482
	public enum KeyCode
	{
		// Token: 0x04000684 RID: 1668
		None,
		// Token: 0x04000685 RID: 1669
		Backspace = 8,
		// Token: 0x04000686 RID: 1670
		Delete = 127,
		// Token: 0x04000687 RID: 1671
		Tab = 9,
		// Token: 0x04000688 RID: 1672
		Clear = 12,
		// Token: 0x04000689 RID: 1673
		Return,
		// Token: 0x0400068A RID: 1674
		Pause = 19,
		// Token: 0x0400068B RID: 1675
		Escape = 27,
		// Token: 0x0400068C RID: 1676
		Space = 32,
		// Token: 0x0400068D RID: 1677
		Keypad0 = 256,
		// Token: 0x0400068E RID: 1678
		Keypad1,
		// Token: 0x0400068F RID: 1679
		Keypad2,
		// Token: 0x04000690 RID: 1680
		Keypad3,
		// Token: 0x04000691 RID: 1681
		Keypad4,
		// Token: 0x04000692 RID: 1682
		Keypad5,
		// Token: 0x04000693 RID: 1683
		Keypad6,
		// Token: 0x04000694 RID: 1684
		Keypad7,
		// Token: 0x04000695 RID: 1685
		Keypad8,
		// Token: 0x04000696 RID: 1686
		Keypad9,
		// Token: 0x04000697 RID: 1687
		KeypadPeriod,
		// Token: 0x04000698 RID: 1688
		KeypadDivide,
		// Token: 0x04000699 RID: 1689
		KeypadMultiply,
		// Token: 0x0400069A RID: 1690
		KeypadMinus,
		// Token: 0x0400069B RID: 1691
		KeypadPlus,
		// Token: 0x0400069C RID: 1692
		KeypadEnter,
		// Token: 0x0400069D RID: 1693
		KeypadEquals,
		// Token: 0x0400069E RID: 1694
		UpArrow,
		// Token: 0x0400069F RID: 1695
		DownArrow,
		// Token: 0x040006A0 RID: 1696
		RightArrow,
		// Token: 0x040006A1 RID: 1697
		LeftArrow,
		// Token: 0x040006A2 RID: 1698
		Insert,
		// Token: 0x040006A3 RID: 1699
		Home,
		// Token: 0x040006A4 RID: 1700
		End,
		// Token: 0x040006A5 RID: 1701
		PageUp,
		// Token: 0x040006A6 RID: 1702
		PageDown,
		// Token: 0x040006A7 RID: 1703
		F1,
		// Token: 0x040006A8 RID: 1704
		F2,
		// Token: 0x040006A9 RID: 1705
		F3,
		// Token: 0x040006AA RID: 1706
		F4,
		// Token: 0x040006AB RID: 1707
		F5,
		// Token: 0x040006AC RID: 1708
		F6,
		// Token: 0x040006AD RID: 1709
		F7,
		// Token: 0x040006AE RID: 1710
		F8,
		// Token: 0x040006AF RID: 1711
		F9,
		// Token: 0x040006B0 RID: 1712
		F10,
		// Token: 0x040006B1 RID: 1713
		F11,
		// Token: 0x040006B2 RID: 1714
		F12,
		// Token: 0x040006B3 RID: 1715
		F13,
		// Token: 0x040006B4 RID: 1716
		F14,
		// Token: 0x040006B5 RID: 1717
		F15,
		// Token: 0x040006B6 RID: 1718
		Alpha0 = 48,
		// Token: 0x040006B7 RID: 1719
		Alpha1,
		// Token: 0x040006B8 RID: 1720
		Alpha2,
		// Token: 0x040006B9 RID: 1721
		Alpha3,
		// Token: 0x040006BA RID: 1722
		Alpha4,
		// Token: 0x040006BB RID: 1723
		Alpha5,
		// Token: 0x040006BC RID: 1724
		Alpha6,
		// Token: 0x040006BD RID: 1725
		Alpha7,
		// Token: 0x040006BE RID: 1726
		Alpha8,
		// Token: 0x040006BF RID: 1727
		Alpha9,
		// Token: 0x040006C0 RID: 1728
		Exclaim = 33,
		// Token: 0x040006C1 RID: 1729
		DoubleQuote,
		// Token: 0x040006C2 RID: 1730
		Hash,
		// Token: 0x040006C3 RID: 1731
		Dollar,
		// Token: 0x040006C4 RID: 1732
		Percent,
		// Token: 0x040006C5 RID: 1733
		Ampersand,
		// Token: 0x040006C6 RID: 1734
		Quote,
		// Token: 0x040006C7 RID: 1735
		LeftParen,
		// Token: 0x040006C8 RID: 1736
		RightParen,
		// Token: 0x040006C9 RID: 1737
		Asterisk,
		// Token: 0x040006CA RID: 1738
		Plus,
		// Token: 0x040006CB RID: 1739
		Comma,
		// Token: 0x040006CC RID: 1740
		Minus,
		// Token: 0x040006CD RID: 1741
		Period,
		// Token: 0x040006CE RID: 1742
		Slash,
		// Token: 0x040006CF RID: 1743
		Colon = 58,
		// Token: 0x040006D0 RID: 1744
		Semicolon,
		// Token: 0x040006D1 RID: 1745
		Less,
		// Token: 0x040006D2 RID: 1746
		Equals,
		// Token: 0x040006D3 RID: 1747
		Greater,
		// Token: 0x040006D4 RID: 1748
		Question,
		// Token: 0x040006D5 RID: 1749
		At,
		// Token: 0x040006D6 RID: 1750
		LeftBracket = 91,
		// Token: 0x040006D7 RID: 1751
		Backslash,
		// Token: 0x040006D8 RID: 1752
		RightBracket,
		// Token: 0x040006D9 RID: 1753
		Caret,
		// Token: 0x040006DA RID: 1754
		Underscore,
		// Token: 0x040006DB RID: 1755
		BackQuote,
		// Token: 0x040006DC RID: 1756
		A,
		// Token: 0x040006DD RID: 1757
		B,
		// Token: 0x040006DE RID: 1758
		C,
		// Token: 0x040006DF RID: 1759
		D,
		// Token: 0x040006E0 RID: 1760
		E,
		// Token: 0x040006E1 RID: 1761
		F,
		// Token: 0x040006E2 RID: 1762
		G,
		// Token: 0x040006E3 RID: 1763
		H,
		// Token: 0x040006E4 RID: 1764
		I,
		// Token: 0x040006E5 RID: 1765
		J,
		// Token: 0x040006E6 RID: 1766
		K,
		// Token: 0x040006E7 RID: 1767
		L,
		// Token: 0x040006E8 RID: 1768
		M,
		// Token: 0x040006E9 RID: 1769
		N,
		// Token: 0x040006EA RID: 1770
		O,
		// Token: 0x040006EB RID: 1771
		P,
		// Token: 0x040006EC RID: 1772
		Q,
		// Token: 0x040006ED RID: 1773
		R,
		// Token: 0x040006EE RID: 1774
		S,
		// Token: 0x040006EF RID: 1775
		T,
		// Token: 0x040006F0 RID: 1776
		U,
		// Token: 0x040006F1 RID: 1777
		V,
		// Token: 0x040006F2 RID: 1778
		W,
		// Token: 0x040006F3 RID: 1779
		X,
		// Token: 0x040006F4 RID: 1780
		Y,
		// Token: 0x040006F5 RID: 1781
		Z,
		// Token: 0x040006F6 RID: 1782
		LeftCurlyBracket,
		// Token: 0x040006F7 RID: 1783
		Pipe,
		// Token: 0x040006F8 RID: 1784
		RightCurlyBracket,
		// Token: 0x040006F9 RID: 1785
		Tilde,
		// Token: 0x040006FA RID: 1786
		Numlock = 300,
		// Token: 0x040006FB RID: 1787
		CapsLock,
		// Token: 0x040006FC RID: 1788
		ScrollLock,
		// Token: 0x040006FD RID: 1789
		RightShift,
		// Token: 0x040006FE RID: 1790
		LeftShift,
		// Token: 0x040006FF RID: 1791
		RightControl,
		// Token: 0x04000700 RID: 1792
		LeftControl,
		// Token: 0x04000701 RID: 1793
		RightAlt,
		// Token: 0x04000702 RID: 1794
		LeftAlt,
		// Token: 0x04000703 RID: 1795
		LeftMeta = 310,
		// Token: 0x04000704 RID: 1796
		LeftCommand = 310,
		// Token: 0x04000705 RID: 1797
		LeftApple = 310,
		// Token: 0x04000706 RID: 1798
		LeftWindows,
		// Token: 0x04000707 RID: 1799
		RightMeta = 309,
		// Token: 0x04000708 RID: 1800
		RightCommand = 309,
		// Token: 0x04000709 RID: 1801
		RightApple = 309,
		// Token: 0x0400070A RID: 1802
		RightWindows = 312,
		// Token: 0x0400070B RID: 1803
		AltGr,
		// Token: 0x0400070C RID: 1804
		Help = 315,
		// Token: 0x0400070D RID: 1805
		Print,
		// Token: 0x0400070E RID: 1806
		SysReq,
		// Token: 0x0400070F RID: 1807
		Break,
		// Token: 0x04000710 RID: 1808
		Menu,
		// Token: 0x04000711 RID: 1809
		Mouse0 = 323,
		// Token: 0x04000712 RID: 1810
		Mouse1,
		// Token: 0x04000713 RID: 1811
		Mouse2,
		// Token: 0x04000714 RID: 1812
		Mouse3,
		// Token: 0x04000715 RID: 1813
		Mouse4,
		// Token: 0x04000716 RID: 1814
		Mouse5,
		// Token: 0x04000717 RID: 1815
		Mouse6,
		// Token: 0x04000718 RID: 1816
		JoystickButton0,
		// Token: 0x04000719 RID: 1817
		JoystickButton1,
		// Token: 0x0400071A RID: 1818
		JoystickButton2,
		// Token: 0x0400071B RID: 1819
		JoystickButton3,
		// Token: 0x0400071C RID: 1820
		JoystickButton4,
		// Token: 0x0400071D RID: 1821
		JoystickButton5,
		// Token: 0x0400071E RID: 1822
		JoystickButton6,
		// Token: 0x0400071F RID: 1823
		JoystickButton7,
		// Token: 0x04000720 RID: 1824
		JoystickButton8,
		// Token: 0x04000721 RID: 1825
		JoystickButton9,
		// Token: 0x04000722 RID: 1826
		JoystickButton10,
		// Token: 0x04000723 RID: 1827
		JoystickButton11,
		// Token: 0x04000724 RID: 1828
		JoystickButton12,
		// Token: 0x04000725 RID: 1829
		JoystickButton13,
		// Token: 0x04000726 RID: 1830
		JoystickButton14,
		// Token: 0x04000727 RID: 1831
		JoystickButton15,
		// Token: 0x04000728 RID: 1832
		JoystickButton16,
		// Token: 0x04000729 RID: 1833
		JoystickButton17,
		// Token: 0x0400072A RID: 1834
		JoystickButton18,
		// Token: 0x0400072B RID: 1835
		JoystickButton19,
		// Token: 0x0400072C RID: 1836
		Joystick1Button0,
		// Token: 0x0400072D RID: 1837
		Joystick1Button1,
		// Token: 0x0400072E RID: 1838
		Joystick1Button2,
		// Token: 0x0400072F RID: 1839
		Joystick1Button3,
		// Token: 0x04000730 RID: 1840
		Joystick1Button4,
		// Token: 0x04000731 RID: 1841
		Joystick1Button5,
		// Token: 0x04000732 RID: 1842
		Joystick1Button6,
		// Token: 0x04000733 RID: 1843
		Joystick1Button7,
		// Token: 0x04000734 RID: 1844
		Joystick1Button8,
		// Token: 0x04000735 RID: 1845
		Joystick1Button9,
		// Token: 0x04000736 RID: 1846
		Joystick1Button10,
		// Token: 0x04000737 RID: 1847
		Joystick1Button11,
		// Token: 0x04000738 RID: 1848
		Joystick1Button12,
		// Token: 0x04000739 RID: 1849
		Joystick1Button13,
		// Token: 0x0400073A RID: 1850
		Joystick1Button14,
		// Token: 0x0400073B RID: 1851
		Joystick1Button15,
		// Token: 0x0400073C RID: 1852
		Joystick1Button16,
		// Token: 0x0400073D RID: 1853
		Joystick1Button17,
		// Token: 0x0400073E RID: 1854
		Joystick1Button18,
		// Token: 0x0400073F RID: 1855
		Joystick1Button19,
		// Token: 0x04000740 RID: 1856
		Joystick2Button0,
		// Token: 0x04000741 RID: 1857
		Joystick2Button1,
		// Token: 0x04000742 RID: 1858
		Joystick2Button2,
		// Token: 0x04000743 RID: 1859
		Joystick2Button3,
		// Token: 0x04000744 RID: 1860
		Joystick2Button4,
		// Token: 0x04000745 RID: 1861
		Joystick2Button5,
		// Token: 0x04000746 RID: 1862
		Joystick2Button6,
		// Token: 0x04000747 RID: 1863
		Joystick2Button7,
		// Token: 0x04000748 RID: 1864
		Joystick2Button8,
		// Token: 0x04000749 RID: 1865
		Joystick2Button9,
		// Token: 0x0400074A RID: 1866
		Joystick2Button10,
		// Token: 0x0400074B RID: 1867
		Joystick2Button11,
		// Token: 0x0400074C RID: 1868
		Joystick2Button12,
		// Token: 0x0400074D RID: 1869
		Joystick2Button13,
		// Token: 0x0400074E RID: 1870
		Joystick2Button14,
		// Token: 0x0400074F RID: 1871
		Joystick2Button15,
		// Token: 0x04000750 RID: 1872
		Joystick2Button16,
		// Token: 0x04000751 RID: 1873
		Joystick2Button17,
		// Token: 0x04000752 RID: 1874
		Joystick2Button18,
		// Token: 0x04000753 RID: 1875
		Joystick2Button19,
		// Token: 0x04000754 RID: 1876
		Joystick3Button0,
		// Token: 0x04000755 RID: 1877
		Joystick3Button1,
		// Token: 0x04000756 RID: 1878
		Joystick3Button2,
		// Token: 0x04000757 RID: 1879
		Joystick3Button3,
		// Token: 0x04000758 RID: 1880
		Joystick3Button4,
		// Token: 0x04000759 RID: 1881
		Joystick3Button5,
		// Token: 0x0400075A RID: 1882
		Joystick3Button6,
		// Token: 0x0400075B RID: 1883
		Joystick3Button7,
		// Token: 0x0400075C RID: 1884
		Joystick3Button8,
		// Token: 0x0400075D RID: 1885
		Joystick3Button9,
		// Token: 0x0400075E RID: 1886
		Joystick3Button10,
		// Token: 0x0400075F RID: 1887
		Joystick3Button11,
		// Token: 0x04000760 RID: 1888
		Joystick3Button12,
		// Token: 0x04000761 RID: 1889
		Joystick3Button13,
		// Token: 0x04000762 RID: 1890
		Joystick3Button14,
		// Token: 0x04000763 RID: 1891
		Joystick3Button15,
		// Token: 0x04000764 RID: 1892
		Joystick3Button16,
		// Token: 0x04000765 RID: 1893
		Joystick3Button17,
		// Token: 0x04000766 RID: 1894
		Joystick3Button18,
		// Token: 0x04000767 RID: 1895
		Joystick3Button19,
		// Token: 0x04000768 RID: 1896
		Joystick4Button0,
		// Token: 0x04000769 RID: 1897
		Joystick4Button1,
		// Token: 0x0400076A RID: 1898
		Joystick4Button2,
		// Token: 0x0400076B RID: 1899
		Joystick4Button3,
		// Token: 0x0400076C RID: 1900
		Joystick4Button4,
		// Token: 0x0400076D RID: 1901
		Joystick4Button5,
		// Token: 0x0400076E RID: 1902
		Joystick4Button6,
		// Token: 0x0400076F RID: 1903
		Joystick4Button7,
		// Token: 0x04000770 RID: 1904
		Joystick4Button8,
		// Token: 0x04000771 RID: 1905
		Joystick4Button9,
		// Token: 0x04000772 RID: 1906
		Joystick4Button10,
		// Token: 0x04000773 RID: 1907
		Joystick4Button11,
		// Token: 0x04000774 RID: 1908
		Joystick4Button12,
		// Token: 0x04000775 RID: 1909
		Joystick4Button13,
		// Token: 0x04000776 RID: 1910
		Joystick4Button14,
		// Token: 0x04000777 RID: 1911
		Joystick4Button15,
		// Token: 0x04000778 RID: 1912
		Joystick4Button16,
		// Token: 0x04000779 RID: 1913
		Joystick4Button17,
		// Token: 0x0400077A RID: 1914
		Joystick4Button18,
		// Token: 0x0400077B RID: 1915
		Joystick4Button19,
		// Token: 0x0400077C RID: 1916
		Joystick5Button0,
		// Token: 0x0400077D RID: 1917
		Joystick5Button1,
		// Token: 0x0400077E RID: 1918
		Joystick5Button2,
		// Token: 0x0400077F RID: 1919
		Joystick5Button3,
		// Token: 0x04000780 RID: 1920
		Joystick5Button4,
		// Token: 0x04000781 RID: 1921
		Joystick5Button5,
		// Token: 0x04000782 RID: 1922
		Joystick5Button6,
		// Token: 0x04000783 RID: 1923
		Joystick5Button7,
		// Token: 0x04000784 RID: 1924
		Joystick5Button8,
		// Token: 0x04000785 RID: 1925
		Joystick5Button9,
		// Token: 0x04000786 RID: 1926
		Joystick5Button10,
		// Token: 0x04000787 RID: 1927
		Joystick5Button11,
		// Token: 0x04000788 RID: 1928
		Joystick5Button12,
		// Token: 0x04000789 RID: 1929
		Joystick5Button13,
		// Token: 0x0400078A RID: 1930
		Joystick5Button14,
		// Token: 0x0400078B RID: 1931
		Joystick5Button15,
		// Token: 0x0400078C RID: 1932
		Joystick5Button16,
		// Token: 0x0400078D RID: 1933
		Joystick5Button17,
		// Token: 0x0400078E RID: 1934
		Joystick5Button18,
		// Token: 0x0400078F RID: 1935
		Joystick5Button19,
		// Token: 0x04000790 RID: 1936
		Joystick6Button0,
		// Token: 0x04000791 RID: 1937
		Joystick6Button1,
		// Token: 0x04000792 RID: 1938
		Joystick6Button2,
		// Token: 0x04000793 RID: 1939
		Joystick6Button3,
		// Token: 0x04000794 RID: 1940
		Joystick6Button4,
		// Token: 0x04000795 RID: 1941
		Joystick6Button5,
		// Token: 0x04000796 RID: 1942
		Joystick6Button6,
		// Token: 0x04000797 RID: 1943
		Joystick6Button7,
		// Token: 0x04000798 RID: 1944
		Joystick6Button8,
		// Token: 0x04000799 RID: 1945
		Joystick6Button9,
		// Token: 0x0400079A RID: 1946
		Joystick6Button10,
		// Token: 0x0400079B RID: 1947
		Joystick6Button11,
		// Token: 0x0400079C RID: 1948
		Joystick6Button12,
		// Token: 0x0400079D RID: 1949
		Joystick6Button13,
		// Token: 0x0400079E RID: 1950
		Joystick6Button14,
		// Token: 0x0400079F RID: 1951
		Joystick6Button15,
		// Token: 0x040007A0 RID: 1952
		Joystick6Button16,
		// Token: 0x040007A1 RID: 1953
		Joystick6Button17,
		// Token: 0x040007A2 RID: 1954
		Joystick6Button18,
		// Token: 0x040007A3 RID: 1955
		Joystick6Button19,
		// Token: 0x040007A4 RID: 1956
		Joystick7Button0,
		// Token: 0x040007A5 RID: 1957
		Joystick7Button1,
		// Token: 0x040007A6 RID: 1958
		Joystick7Button2,
		// Token: 0x040007A7 RID: 1959
		Joystick7Button3,
		// Token: 0x040007A8 RID: 1960
		Joystick7Button4,
		// Token: 0x040007A9 RID: 1961
		Joystick7Button5,
		// Token: 0x040007AA RID: 1962
		Joystick7Button6,
		// Token: 0x040007AB RID: 1963
		Joystick7Button7,
		// Token: 0x040007AC RID: 1964
		Joystick7Button8,
		// Token: 0x040007AD RID: 1965
		Joystick7Button9,
		// Token: 0x040007AE RID: 1966
		Joystick7Button10,
		// Token: 0x040007AF RID: 1967
		Joystick7Button11,
		// Token: 0x040007B0 RID: 1968
		Joystick7Button12,
		// Token: 0x040007B1 RID: 1969
		Joystick7Button13,
		// Token: 0x040007B2 RID: 1970
		Joystick7Button14,
		// Token: 0x040007B3 RID: 1971
		Joystick7Button15,
		// Token: 0x040007B4 RID: 1972
		Joystick7Button16,
		// Token: 0x040007B5 RID: 1973
		Joystick7Button17,
		// Token: 0x040007B6 RID: 1974
		Joystick7Button18,
		// Token: 0x040007B7 RID: 1975
		Joystick7Button19,
		// Token: 0x040007B8 RID: 1976
		Joystick8Button0,
		// Token: 0x040007B9 RID: 1977
		Joystick8Button1,
		// Token: 0x040007BA RID: 1978
		Joystick8Button2,
		// Token: 0x040007BB RID: 1979
		Joystick8Button3,
		// Token: 0x040007BC RID: 1980
		Joystick8Button4,
		// Token: 0x040007BD RID: 1981
		Joystick8Button5,
		// Token: 0x040007BE RID: 1982
		Joystick8Button6,
		// Token: 0x040007BF RID: 1983
		Joystick8Button7,
		// Token: 0x040007C0 RID: 1984
		Joystick8Button8,
		// Token: 0x040007C1 RID: 1985
		Joystick8Button9,
		// Token: 0x040007C2 RID: 1986
		Joystick8Button10,
		// Token: 0x040007C3 RID: 1987
		Joystick8Button11,
		// Token: 0x040007C4 RID: 1988
		Joystick8Button12,
		// Token: 0x040007C5 RID: 1989
		Joystick8Button13,
		// Token: 0x040007C6 RID: 1990
		Joystick8Button14,
		// Token: 0x040007C7 RID: 1991
		Joystick8Button15,
		// Token: 0x040007C8 RID: 1992
		Joystick8Button16,
		// Token: 0x040007C9 RID: 1993
		Joystick8Button17,
		// Token: 0x040007CA RID: 1994
		Joystick8Button18,
		// Token: 0x040007CB RID: 1995
		Joystick8Button19
	}
}
